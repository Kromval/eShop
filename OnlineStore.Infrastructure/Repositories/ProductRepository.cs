using Microsoft.EntityFrameworkCore;
using Npgsql;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return await _dbContext.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(p => p.Category)
                    .ToListAsync();
            }

            // Use tsvector for full-text search
            var parameter = new NpgsqlParameter("@searchTerm", searchTerm);
            
            return await _dbContext.Products
                .FromSqlRaw("SELECT * FROM products WHERE search_vector @@ to_tsquery('english', @searchTerm) AND is_active = true", parameter)
                .OrderByDescending(p => EF.Functions.Random()) // This is just a placeholder, actual ranking would be done in SQL
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId, int pageNumber, int pageSize)
        {
            return await _dbContext.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<int> GetTotalProductCountAsync(string searchTerm = null, Guid? categoryId = null)
        {
            IQueryable<Product> query = _dbContext.Products.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var parameter = new NpgsqlParameter("@searchTerm", searchTerm);
                query = _dbContext.Products
                    .FromSqlRaw("SELECT * FROM products WHERE search_vector @@ to_tsquery('english', @searchTerm) AND is_active = true", parameter);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            return await query.CountAsync();
        }
    }
}
