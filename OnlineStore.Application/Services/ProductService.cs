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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductSearchResult> SearchProductsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm, pageNumber, pageSize);
            var totalCount = await _productRepository.GetTotalProductCountAsync(searchTerm);

            return new ProductSearchResult
            {
                Products = _mapper.Map<List<ProductDto>>(products),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductSearchResult> GetProductsByCategoryAsync(Guid categoryId, int pageNumber, int pageSize)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId, pageNumber, pageSize);
            var totalCount = await _productRepository.GetTotalProductCountAsync(null, categoryId);

            return new ProductSearchResult
            {
                Products = _mapper.Map<List<ProductDto>>(products),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                CategoryId = productDto.CategoryId,
                ImageUrl = productDto.ImageUrl,
                IsActive = productDto.IsActive
            };

            await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(ProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(productDto.Id);
            if (product == null)
            {
                throw new Exception($"Product with ID {productDto.Id} not found.");
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.CategoryId = productDto.CategoryId;
            product.ImageUrl = productDto.ImageUrl;
            product.IsActive = productDto.IsActive;

            await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception($"Product with ID {id} not found.");
            }

            await _productRepository.DeleteAsync(product);
        }

        public async Task<bool> IsProductInStockAsync(Guid id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null && product.IsActive && product.StockQuantity >= quantity;
        }
    }
}