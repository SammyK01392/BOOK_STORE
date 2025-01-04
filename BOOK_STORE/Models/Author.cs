namespace BOOK_STORE.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }

        // Relationship
        public ICollection<Book> Books { get; set; }
    }
}
