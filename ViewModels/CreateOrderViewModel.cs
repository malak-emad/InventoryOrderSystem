using System.ComponentModel.DataAnnotations;

namespace InventoryOrderSystem.ViewModels
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Please select a product")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a product")]
        public int? ProductId {get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity {get; set; }
    }
}