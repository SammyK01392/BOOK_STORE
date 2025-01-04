namespace BOOK_STORE.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Relationship
        public ICollection<Book> Books { get; set; }
    }
}
