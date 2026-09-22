using InventoryOrderSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryOrderSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryOrderSystem.Controllers
{
    [Authorize (Roles="Admin")]
    public class ProductsController(ApplicationDbContext applicationDb) : Controller
    {
        // 1- View product list
        // List all products (Admin sees all, active or not)
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var products = await applicationDb.Products.ToListAsync();
            return View(products);
        }

        // 2- View product details
        // Show one product's full info
        [HttpGet]
        public async Task<ActionResult> Details(int Id)
        {
            var product = await applicationDb.Products.Include(p=> p.Category).FirstOrDefaultAsync(p=> p.CategoryId == Id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // 3- Create product
        // Empty form, with a category dropdown
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var newProduct = new Product();

            ViewBag.Categories = new SelectList( await applicationDb.Categories.ToListAsync(), "Id","Name");

            return View(newProduct);
        }

        // Validate, check SKU uniqueness, save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                bool skuExists = await applicationDb.Products.AnyAsync(p => p.SKU == product.SKU);
                if(skuExists)
                {
                    ModelState.AddModelError("SKU", "SKU already exists");
                    ViewBag.Categories = new SelectList( await applicationDb.Categories.ToListAsync(), "Id","Name", product.CategoryId);
                    return View(product);
                }
                product.CreatedDate = DateTime.UtcNow;
                applicationDb.Products.Add(product);
                await applicationDb.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList( await applicationDb.Categories.ToListAsync(), "Id","Name", product.CategoryId);
            return View(product);
        }

        // 4- Edit product
        // Form pre-filled, with the dropdown pre-selected
        [HttpGet]
        public async Task<ActionResult> Edit(int Id)
        {
            var product = await applicationDb.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList( await applicationDb.Categories.ToListAsync(), "Id","Name", product.CategoryId);
            return View(product);
        }

        // Validate, check SKU uniqueness (excluding itself), save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int Id, Product product)
        {
            if (ModelState.IsValid)
            {
                var productEdited = await applicationDb.Products.FindAsync(Id);
                if (productEdited == null)
                {
                    return NotFound();
                }
                var skuExists = await applicationDb.Products.AnyAsync(p=> p.SKU == product.SKU && p.Id != Id);
                if(skuExists)
                {
                    ModelState.AddModelError("SKU", "SKU already exists");
                    ViewBag.Categories = new SelectList( await applicationDb.Categories.ToListAsync(), "Id","Name", product.CategoryId);
                    return View(product);
                }
                productEdited.Name = product.Name;
                var validateCategory = await applicationDb.Categories.FindAsync(product.CategoryId);
                if (validateCategory != null)
                {
                    productEdited.CategoryId = product.CategoryId;
                }
                else
                {
                    ModelState.AddModelError("Category", "Category doesn't Exist");
                    ViewBag.Categories = new SelectList( await applicationDb.Categories.ToListAsync(), "Id","Name", product.CategoryId);
                    return View(product);
                }
                productEdited.Description = product.Description;
                productEdited.IsActive = product.IsActive;
                productEdited.Price = product.Price;
                productEdited.QuantityInStock = product.QuantityInStock;
                productEdited.SKU = product.SKU;
                await applicationDb.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList( await applicationDb.Categories.ToListAsync(), "Id","Name", product.CategoryId);
            return View(product);

        }
        // 5- Delete product
        // Confirmation page
        [HttpGet]
        public async Task<ActionResult> Delete(int Id)
        {
            var product = await applicationDb.Products.FindAsync(Id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        
        // Actually delete, with try/catch for products in existing orders
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int Id)
        {
            var product = await applicationDb.Products.FindAsync(Id);
            if(product == null)
            {
                return NotFound();
            }
            try
            {
                applicationDb.Products.Remove(product);
                await applicationDb.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this product");
                return View(product);
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}