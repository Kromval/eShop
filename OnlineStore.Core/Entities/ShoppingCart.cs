using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public required User User { get; set; }
        public required ICollection<CartItem> CartItems { get; set; }
    }
}