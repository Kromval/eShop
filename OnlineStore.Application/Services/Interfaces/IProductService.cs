using OnlineStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Application.Services.Interfaces
{
    public class ProductSearchResult
    {
        public List<ProductDto> Products { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(Guid id);
        Task<ProductSearchResult> SearchProductsAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ProductSearchResult> GetProductsByCategoryAsync(Guid categoryId, int pageNumber, int pageSize);
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        Task<ProductDto> UpdateProductAsync(ProductDto productDto);
        Task DeleteProductAsync(Guid id);
        Task<bool> IsProductInStockAsync(Guid id, int quantity);
    }
}