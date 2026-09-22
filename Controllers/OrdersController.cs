using InventoryOrderSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InventoryOrderSystem.Models;
using InventoryOrderSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryOrderSystem.Controllers
{
    [Authorize]
    public class OrdersController(ApplicationDbContext applicationDb, UserManager<ApplicationUser> userManager) : Controller
    {
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var ActiveProducts = await applicationDb.Products.Where(p=> p.IsActive == true).ToListAsync();
            ViewBag.Products = new SelectList( ActiveProducts, "Id","Name");
            return View(new CreateOrderViewModel());
        }

        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Create(CreateOrderViewModel model)
        {
            if(ModelState.IsValid)
            {
                var product = await applicationDb.Products.FirstOrDefaultAsync(p=> p.Id == model.ProductId);
                if(product == null)
                {
                    return NotFound();
                }
                var availableQuantity = product.QuantityInStock;
                if(model.Quantity > availableQuantity)
                {
                    ModelState.AddModelError("ProductId", "Insufficient stock");
                    var _activeProducts = await applicationDb.Products.Where(p=> p.IsActive == true).ToListAsync();
                    ViewBag.Products = new SelectList( _activeProducts, "Id","Name", model.ProductId);
                    return View(model);
                }
                var price = product.Price;
                var totalCost = price * model.Quantity;
                product.QuantityInStock = availableQuantity - model.Quantity;

                // Build order
                Order order = new Order();
                order.Status = OrderStatus.Pending;
                order.OrderNumber = $"ORD-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
                order.CreatedByUserId = userManager.GetUserId(User)!;
                order.CreatedDate = DateTime.UtcNow;
                order.TotalAmount = totalCost;
                
                OrderItem orderItem = new OrderItem();
                orderItem.ProductId = model.ProductId!.Value;
                orderItem.Quantity = model.Quantity;
                orderItem.UnitPrice = price;
                order.OrderItems.Add(orderItem);
                applicationDb.Orders.Add(order);

                await applicationDb.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = order.Id });

            }
            var ActiveProducts = await applicationDb.Products.Where(p=> p.IsActive == true).ToListAsync();
            ViewBag.Products = new SelectList( ActiveProducts, "Id","Name", model.ProductId);        
            return View(model);
        }

        // List orders: all of them for Admin, only the logged-in user's own for everyone else
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if(User.IsInRole("Admin"))
            {
                var orders = await applicationDb.Orders
                    .Include(o=> o.CreatedByUser)
                    .OrderByDescending(o => o.CreatedDate)
                    .ToListAsync();
                return View(orders);
            }
            else
            {
                var userId = userManager.GetUserId(User);
                var orders = await applicationDb.Orders
                    .Where(o=> o.CreatedByUserId == userId)
                    .Include(o=> o.CreatedByUser)
                    .OrderByDescending(o => o.CreatedDate)
                    .ToListAsync();
                return View(orders);
            }
        }

        //Show one order's full breakdown, blocked if it's not yours and you're not Admin
        [HttpGet]
        public async Task<ActionResult> Details(int Id)
        {
            var order = await applicationDb.Orders
                .Include(o=> o.CreatedByUser)
                .Include(o=> o.OrderItems).ThenInclude(oi=> oi.Product)
                .FirstOrDefaultAsync(o=> o.Id == Id);

            if(order == null)
            {
                return NotFound();
            }
            if (!(User.IsInRole("Admin") || (userManager.GetUserId(User) == order.CreatedByUserId)))
            {
                return Forbid();
            }
            return View(order);
        }

        // Admin can change order status to Confirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Confirm(int id)
        {
            var order = await applicationDb.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = OrderStatus.Confirmed;
            await applicationDb.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // Admin can change order status to Cancelled
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Cancel(int id)
        {
            var order = await applicationDb.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = OrderStatus.Cancelled;
            await applicationDb.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}