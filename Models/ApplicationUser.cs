using Microsoft.AspNetCore.Identity;

namespace InventoryOrderSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Order> Orders {get; set; } = new List<Order>();
    }
}