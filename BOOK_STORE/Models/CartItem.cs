namespace BOOK_STORE.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }

        // Relationships
        public User User { get; set; }
        public Book Book { get; set; }
    }
}
