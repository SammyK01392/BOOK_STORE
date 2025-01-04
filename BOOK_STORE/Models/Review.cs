namespace BOOK_STORE.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; } // Rating out of 5

        // Relationships
        public Book Book { get; set; }
        public User User { get; set; }
    }
}
