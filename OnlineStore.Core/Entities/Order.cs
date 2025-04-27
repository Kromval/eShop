using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Status { get; set; } // Pending, Processing, Shipped, Delivered, Cancelled
        public required decimal TotalAmount { get; set; }
        public required string ShippingAddress { get; set; }
        public required string BillingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}