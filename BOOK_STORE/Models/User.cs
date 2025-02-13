﻿namespace BOOK_STORE.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Example roles: Admin, Customer

        // Relationship
        public ICollection<Order> Orders { get; set; }
    }
}
