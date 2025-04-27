using OnlineStore.Core.Entities;
using System;
using System.Threading.Tasks;

namespace OnlineStore.Core.Interfaces
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> GetCartWithItemsByUserIdAsync(Guid userId);
        Task<CartItem> GetCartItemAsync(Guid cartId, Guid productId);
    }
}