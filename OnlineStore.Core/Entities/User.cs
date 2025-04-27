using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } // User, Manager, Admin
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<Order> Orders { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public ICollection<ProductReview> Reviews { get; set; }
    }
}