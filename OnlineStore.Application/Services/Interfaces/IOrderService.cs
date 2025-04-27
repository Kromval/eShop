using OnlineStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(Guid id);
        Task<IReadOnlyList<OrderDto>> GetOrdersByUserIdAsync(Guid userId);
        Task<IReadOnlyList<OrderDto>> GetOrdersByStatusAsync(string status, int pageNumber, int pageSize);
        Task<OrderDto> CreateOrderAsync(Guid userId, string shippingAddress, string billingAddress);
        Task<OrderDto> UpdateOrderStatusAsync(Guid id, string status);
    }
}