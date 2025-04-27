using OnlineStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Core.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm, int pageNumber, int pageSize);
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId, int pageNumber, int pageSize);
        Task<int> GetTotalProductCountAsync(string searchTerm = null, Guid? categoryId = null);
    }
}