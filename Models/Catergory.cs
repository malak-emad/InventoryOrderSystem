using System.ComponentModel.DataAnnotations;
namespace InventoryOrderSystem.Models
{
    public class Category
    {
        public int Id {get; set;}

        // Category name should be required
        [Required(ErrorMessage = "Category Name is required")]
        public string Name {get; set;} = string.Empty;
        public string? Description {get; set;}
        public List<Product> Products {get; set;} = new List<Product>();
    }
}