using System;

namespace OnlineStore.Core.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public required int Quantity { get; set; }
        public required decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}