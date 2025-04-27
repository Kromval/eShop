using AutoMapper;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Services.Interfaces;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace OnlineStore.Application.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ShoppingCartDto> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    User = null,
                    CartItems = null
                };
                await _cartRepository.AddAsync(cart);
            }

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task<ShoppingCartDto> AddItemToCartAsync(Guid userId, Guid productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
            {
                throw new Exception("Product not found or not available.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new Exception("Not enough stock available.");
            }

            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    User = null,
                    CartItems = null
                };
                await _cartRepository.AddAsync(cart);
            }

            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(cartItem);
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Cart = null,
                    Product = null
                };
                await _cartRepository.AddAsync(cartItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);

            return await GetCartByUserIdAsync(userId);
        }

        public async Task<ShoppingCartDto> UpdateCartItemQuantityAsync(Guid userId, Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                return await RemoveItemFromCartAsync(userId, productId);
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
            {
                throw new Exception("Product not found or not available.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new Exception("Not enough stock available.");
            }

            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                throw new Exception("Shopping cart not found.");
            }

            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
            {
                throw new Exception("Item not found in cart.");
            }

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cartItem);

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);

            return await GetCartByUserIdAsync(userId);
        }

        public async Task<ShoppingCartDto> RemoveItemFromCartAsync(Guid userId, Guid productId)
        {
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                throw new Exception("Shopping cart not found.");
            }

            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem != null)
            {
                await _cartRepository.DeleteAsync(cartItem);

                cart.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(cart);
            }

            return await GetCartByUserIdAsync(userId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                return;
            }

            foreach (var item in cart.CartItems)
            {
                await _cartRepository.DeleteAsync(item);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);
        }
    }
}