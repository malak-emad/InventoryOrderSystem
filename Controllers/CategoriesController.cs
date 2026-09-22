using InventoryOrderSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryOrderSystem.Models;

namespace InventoryOrderSystem.Controllers
{
    [Authorize (Roles="Admin")]
    public class CategoriesController(ApplicationDbContext applicationDb) : Controller
    {
        // 1- Loads all categories and show them in a table (View category list)
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var categories = await applicationDb.Categories.ToListAsync();
            return View(categories);
        }

        // 2- Create category
        // Shows an empty form
        [HttpGet]
        public IActionResult Create()
        {
            var newCategory = new Category();
            return View(newCategory);
        }

        // Receives the form, validates, saves a new row
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                applicationDb.Categories.Add(category);
                await applicationDb.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 3- Edit category
        // Loads one category by id, shows the form pre-filled
        [HttpGet]
        public async Task<ActionResult> Edit(int Id)
        {
            var category = await applicationDb.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Receives the form, validates, updates that row
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int Id, Category category)
        {
            if(ModelState.IsValid)
            {
                var categoryEdited = await applicationDb.Categories.FindAsync(Id);
                if(categoryEdited == null)
                {
                    return NotFound();
                }
                categoryEdited.Name = category.Name;
                categoryEdited.Description = category.Description;
                await applicationDb.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 4- Delete category
        // Loads one category, shows a "are you sure?" confirmation page
        [HttpGet]
        public async Task<ActionResult> Delete(int Id)
        {
            var category = await applicationDb.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);   // are you sure message
        }

        // Actually deletes the row
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int Id)
        {
            var category = await applicationDb.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }
            try
            {            
                applicationDb.Categories.Remove(category);
                await applicationDb.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this category because it has products assigned to it");
                return View(category);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}