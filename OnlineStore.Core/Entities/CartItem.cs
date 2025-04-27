using System;

namespace OnlineStore.Core.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public required int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public required ShoppingCart Cart { get; set; }
        public required Product Product { get; set; }
    }
}