using OnlineStore.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace OnlineStore.Application.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> GetCartByUserIdAsync(Guid userId);
        Task<ShoppingCartDto> AddItemToCartAsync(Guid userId, Guid productId, int quantity);
        Task<ShoppingCartDto> UpdateCartItemQuantityAsync(Guid userId, Guid productId, int quantity);
        Task<ShoppingCartDto> RemoveItemFromCartAsync(Guid userId, Guid productId);
        Task ClearCartAsync(Guid userId);
    }
}