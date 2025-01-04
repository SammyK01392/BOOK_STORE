namespace BOOK_STORE.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } // Example: Credit Card, PayPal
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        // Relationships
        public Order Order { get; set; }
    }
}
