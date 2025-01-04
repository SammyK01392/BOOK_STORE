using BOOK_STORE.Models;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace BOOK_STORE.Services
{
    public class UserManager
    {
        private readonly BookStoreDbContext _context;

        public UserManager(BookStoreDbContext context)
        {
            _context = context;
        }

        // Hash the password using SHA256
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Verify the password
        public bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            return HashPassword(inputPassword) == storedPasswordHash;
        }

        // Authenticate a user by email and password
        public User AuthenticateUser(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        // Create a new user with hashed password
        public void CreateUser(string fullName, string email, string password, string role)
        {
            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}

