using System.ComponentModel.DataAnnotations;
namespace InventoryOrderSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        // Name is required
        [Required(ErrorMessage = "Product Name is required")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set;}
        
        // SKU is required
        [Required(ErrorMessage = "SKU is required")]
        public string SKU { get; set; } = string.Empty;
        // Price must be greater than 0
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        // Quantity cannot be negative
        [Range(0, int.MaxValue, ErrorMessage = "Quantity in stock cannot be negative")]
        public int QuantityInStock { get; set; }

        // Category is required
        public Category? Category { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}