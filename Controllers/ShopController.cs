using Microsoft.AspNetCore.Mvc;
using InventoryOrderSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryOrderSystem.Controllers
{
    [Authorize]

    public class ShopController(ApplicationDbContext applicationDb) : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Index(string? name, int? categoryId)
        {
            var products = applicationDb.Products.Where(p=> p.IsActive);
            if(name != null)
            {
                products = products.Where(p=> p.Name.Contains(name));
            }
            if(categoryId.HasValue)
            {
                products = products.Where(p=> p.CategoryId == categoryId);
            }
            var productsList = await products.Include(p => p.Category).ToListAsync();
            ViewBag.Categories = new SelectList(await applicationDb.Categories.ToListAsync(), "Id", "Name", categoryId);
            return View(productsList);
        }
    }
}