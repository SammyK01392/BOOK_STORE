namespace BOOK_STORE.Models
{
    public class MasterView
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Author> authors  { get; set; }

        public IEnumerable<Category> categories { get; set; }

        public IEnumerable<User> users { get; set; }

        public IEnumerable <Review> reviews { get; set; }

        public IEnumerable <Order> orders { get; set; }
    }
}
