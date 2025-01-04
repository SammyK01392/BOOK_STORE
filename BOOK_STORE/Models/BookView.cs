namespace BOOK_STORE.Models
{
    public class BookView
    {
        public int Id { get; set; }
        public string Title { get; set; }
     
        public IFormFile Photo { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }

        // Relationships
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
