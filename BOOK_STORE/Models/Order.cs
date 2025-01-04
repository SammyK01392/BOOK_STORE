namespace BOOK_STORE.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Example: Pending, Shipped, Delivered

        // Relationships
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
