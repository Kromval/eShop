using OnlineStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Core.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<Order> GetOrderWithItemsAsync(Guid orderId);
        Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(string status, int pageNumber, int pageSize);
    }
}