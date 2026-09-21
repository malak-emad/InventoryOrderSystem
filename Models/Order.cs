namespace InventoryOrderSystem.Models
{
    public class Order
    {
        public int Id {get; set;}
        public string OrderNumber {get; set;} = string.Empty;
        public DateTime CreatedDate {get; set;}
        public ApplicationUser? CreatedByUser {get; set;}   // i do not know if this is correct
        // it is string by default because the user id is a string in the IdentityUser class
        public string CreatedByUserId {get; set;} = string.Empty;
        public OrderStatus Status {get; set;} = OrderStatus.Pending;
        public decimal TotalAmount {get; set;}
        public List<OrderItem> OrderItems {get; set;} = new List<OrderItem>();

    }
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}