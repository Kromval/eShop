using AutoMapper;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Services.Interfaces;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IShoppingCartRepository cartRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IReadOnlyList<OrderDto>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return _mapper.Map<IReadOnlyList<OrderDto>>(orders);
        }

        public async Task<IReadOnlyList<OrderDto>> GetOrdersByStatusAsync(string status, int pageNumber, int pageSize)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status, pageNumber, pageSize);
            return _mapper.Map<IReadOnlyList<OrderDto>>(orders);
        }

        public async Task<OrderDto> CreateOrderAsync(Guid userId, string shippingAddress, string billingAddress)
        {
            // Get user's shopping cart
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || cart.CartItems.Count == 0)
            {
                throw new Exception("Shopping cart is empty.");
            }

            // Create new order
            var order = new Order
            {
                UserId = userId,
                Status = "Pending",
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            // Add items from cart to order
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null || !product.IsActive || product.StockQuantity < cartItem.Quantity)
                {
                    throw new Exception($"Product {product?.Name ?? cartItem.ProductId.ToString()} is not available in the requested quantity.");
                }

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.UnitPrice;

                // Update product stock
                product.StockQuantity -= cartItem.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            order.TotalAmount = totalAmount;

            // Save order
            await _orderRepository.AddAsync(order);

            // Clear shopping cart
            foreach (var cartItem in cart.CartItems)
            {
                await _cartRepository.DeleteAsync(cartItem);
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(Guid id, string status)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id);
            if (order == null)
            {
                throw new Exception($"Order with ID {id} not found.");
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }
    }
}