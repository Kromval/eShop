# .NET Web API Backend Development

This section covers the development of the backend for the online store application using .NET Web API, following test-driven development methodology.

## 1. Setting Up the .NET Web API Project

### Prerequisites

- .NET 7.0 SDK or later installed
- Visual Studio 2022 or Visual Studio Code
- PostgreSQL database set up (as described in the previous section)

### Creating the Project

```bash
# Create a new solution
dotnet new sln -n OnlineStore

# Create Web API project
dotnet new webapi -n OnlineStore.API

# Create class library projects
dotnet new classlib -n OnlineStore.Core
dotnet new classlib -n OnlineStore.Infrastructure
dotnet new classlib -n OnlineStore.Application

# Create test projects
dotnet new xunit -n OnlineStore.UnitTests
dotnet new xunit -n OnlineStore.IntegrationTests

# Add projects to solution
dotnet sln add OnlineStore.API/OnlineStore.API.csproj
dotnet sln add OnlineStore.Core/OnlineStore.Core.csproj
dotnet sln add OnlineStore.Infrastructure/OnlineStore.Infrastructure.csproj
dotnet sln add OnlineStore.Application/OnlineStore.Application.csproj
dotnet sln add OnlineStore.UnitTests/OnlineStore.UnitTests.csproj
dotnet sln add OnlineStore.IntegrationTests/OnlineStore.IntegrationTests.csproj

# Add project references
dotnet add OnlineStore.API/OnlineStore.API.csproj reference OnlineStore.Application/OnlineStore.Application.csproj
dotnet add OnlineStore.Application/OnlineStore.Application.csproj reference OnlineStore.Core/OnlineStore.Core.csproj
dotnet add OnlineStore.Infrastructure/OnlineStore.Infrastructure.csproj reference OnlineStore.Core/OnlineStore.Core.csproj
dotnet add OnlineStore.Application/OnlineStore.Application.csproj reference OnlineStore.Infrastructure/OnlineStore.Infrastructure.csproj
dotnet add OnlineStore.UnitTests/OnlineStore.UnitTests.csproj reference OnlineStore.Core/OnlineStore.Core.csproj
dotnet add OnlineStore.UnitTests/OnlineStore.UnitTests.csproj reference OnlineStore.Application/OnlineStore.Application.csproj
dotnet add OnlineStore.IntegrationTests/OnlineStore.IntegrationTests.csproj reference OnlineStore.API/OnlineStore.API.csproj
```

### Installing Required NuGet Packages

```bash
# API project packages
dotnet add OnlineStore.API/OnlineStore.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add OnlineStore.API/OnlineStore.API.csproj package Microsoft.AspNetCore.Identity
dotnet add OnlineStore.API/OnlineStore.API.csproj package Swashbuckle.AspNetCore
dotnet add OnlineStore.API/OnlineStore.API.csproj package Microsoft.EntityFrameworkCore.Design

# Infrastructure project packages
dotnet add OnlineStore.Infrastructure/OnlineStore.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add OnlineStore.Infrastructure/OnlineStore.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add OnlineStore.Infrastructure/OnlineStore.Infrastructure.csproj package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add OnlineStore.Infrastructure/OnlineStore.Infrastructure.csproj package Microsoft.Extensions.Configuration
dotnet add OnlineStore.Infrastructure/OnlineStore.Infrastructure.csproj package Microsoft.Extensions.Options.ConfigurationExtensions

# Application project packages
dotnet add OnlineStore.Application/OnlineStore.Application.csproj package AutoMapper
dotnet add OnlineStore.Application/OnlineStore.Application.csproj package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add OnlineStore.Application/OnlineStore.Application.csproj package FluentValidation
dotnet add OnlineStore.Application/OnlineStore.Application.csproj package FluentValidation.AspNetCore
dotnet add OnlineStore.Application/OnlineStore.Application.csproj package MediatR
dotnet add OnlineStore.Application/OnlineStore.Application.csproj package MediatR.Extensions.Microsoft.DependencyInjection

# Test project packages
dotnet add OnlineStore.UnitTests/OnlineStore.UnitTests.csproj package Moq
dotnet add OnlineStore.UnitTests/OnlineStore.UnitTests.csproj package FluentAssertions
dotnet add OnlineStore.IntegrationTests/OnlineStore.IntegrationTests.csproj package Microsoft.AspNetCore.Mvc.Testing
dotnet add OnlineStore.IntegrationTests/OnlineStore.IntegrationTests.csproj package Microsoft.EntityFrameworkCore.InMemory
dotnet add OnlineStore.IntegrationTests/OnlineStore.IntegrationTests.csproj package FluentAssertions
```

## 2. Project Structure Overview

The backend follows a clean architecture approach with the following layers:

1. **Core Layer** (OnlineStore.Core): Contains domain entities, interfaces, and business logic
2. **Infrastructure Layer** (OnlineStore.Infrastructure): Contains database context, repositories, and external service implementations
3. **Application Layer** (OnlineStore.Application): Contains application services, DTOs, and business logic orchestration
4. **API Layer** (OnlineStore.API): Contains controllers, middleware, and API configuration

## 3. Core Layer Implementation

### Domain Entities

First, let's implement the domain entities in the Core layer:

#### User.cs

```csharp
// OnlineStore.Core/Entities/User.cs
using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } // User, Manager, Admin
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<Order> Orders { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public ICollection<ProductReview> Reviews { get; set; }
    }
}
```

#### Product.cs

```csharp
// OnlineStore.Core/Entities/Product.cs
using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid? CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public Category Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<ProductReview> Reviews { get; set; }
    }
}
```

#### Category.cs

```csharp
// OnlineStore.Core/Entities/Category.cs
using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public Category ParentCategory { get; set; }
        public ICollection<Category> Subcategories { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
```

#### Order.cs

```csharp
// OnlineStore.Core/Entities/Order.cs
using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } // Pending, Processing, Shipped, Delivered, Cancelled
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
```

#### OrderItem.cs

```csharp
// OnlineStore.Core/Entities/OrderItem.cs
using System;

namespace OnlineStore.Core.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
```

#### ShoppingCart.cs

```csharp
// OnlineStore.Core/Entities/ShoppingCart.cs
using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
```

#### CartItem.cs

```csharp
// OnlineStore.Core/Entities/CartItem.cs
using System;

namespace OnlineStore.Core.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public ShoppingCart Cart { get; set; }
        public Product Product { get; set; }
    }
}
```

#### ProductReview.cs

```csharp
// OnlineStore.Core/Entities/ProductReview.cs
using System;

namespace OnlineStore.Core.Entities
{
    public class ProductReview
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public Product Product { get; set; }
        public User User { get; set; }
    }
}
```

### Repository Interfaces

Next, let's define the repository interfaces:

#### IRepository.cs

```csharp
// OnlineStore.Core/Interfaces/IRepository.cs
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineStore.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
```

#### IProductRepository.cs

```csharp
// OnlineStore.Core/Interfaces/IProductRepository.cs
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
```

#### IUserRepository.cs

```csharp
// OnlineStore.Core/Interfaces/IUserRepository.cs
using OnlineStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStore.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role);
    }
}
```

#### IOrderRepository.cs

```csharp
// OnlineStore.Core/Interfaces/IOrderRepository.cs
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
```

#### IShoppingCartRepository.cs

```csharp
// OnlineStore.Core/Interfaces/IShoppingCartRepository.cs
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
```

## 4. Infrastructure Layer Implementation

### Database Context

```csharp
// OnlineStore.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Username).HasColumnName("username").IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.Role).HasColumnName("role").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10, 2)");
                entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.ImageUrl).HasColumnName("image_url");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId);
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.ParentId).HasColumnName("parent_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.ParentCategory)
                    .WithMany(c => c.Subcategories)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Status).HasColumnName("status").IsRequired();
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ShippingAddress).HasColumnName("shipping_address").IsRequired();
                entity.Property(e => e.BillingAddress).HasColumnName("billing_address").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId);
            });

            // Configure OrderItem entity
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(10, 2)");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId);
            });

            // Configure ShoppingCart entity
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.ToTable("shopping_carts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.User)
                    .WithOne(u => u.ShoppingCart)
                    .HasForeignKey<ShoppingCart>(e => e.UserId);
            });

            // Configure CartItem entity
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("cart_items");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CartId).HasColumnName("cart_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(e => e.CartId);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(e => e.ProductId);
            });

            // Configure ProductReview entity
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.ToTable("product_reviews");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ProductId);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.UserId);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update timestamps for entities
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Product product)
                {
                    if (entry.State == EntityState.Added)
                    {
                        product.CreatedAt = DateTime.UtcNow;
                        product.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        product.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else if (entry.Entity is User user)
                {
                    if (entry.State == EntityState.Added)
                    {
                        user.CreatedAt = DateTime.UtcNow;
                        user.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        user.UpdatedAt = DateTime.UtcNow;
                    }
                }
                // Add similar logic for other entities with timestamps
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
```

### Repository Implementations

#### Repository.cs

```csharp
// OnlineStore.Infrastructure/Repositories/Repository.cs
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).CountAsync();
        }
    }
}
```

#### ProductRepository.cs

```csharp
// OnlineStore.Infrastructure/Repositories/ProductRepository.cs
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
```

#### UserRepository.cs

```csharp
// OnlineStore.Infrastructure/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbContext.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }
    }
}
```

#### OrderRepository.cs

```csharp
// OnlineStore.Infrastructure/Repositories/OrderRepository.cs
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> GetOrderWithItemsAsync(Guid orderId)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(string status, int pageNumber, int pageSize)
        {
            return await _dbContext.Orders
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }
    }
}
```

#### ShoppingCartRepository.cs

```csharp
// OnlineStore.Infrastructure/Repositories/ShoppingCartRepository.cs
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace OnlineStore.Infrastructure.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ShoppingCart> GetCartWithItemsByUserIdAsync(Guid userId)
        {
            return await _dbContext.ShoppingCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<CartItem> GetCartItemAsync(Guid cartId, Guid productId)
        {
            return await _dbContext.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }
    }
}
```

### Infrastructure Service Registration

```csharp
// OnlineStore.Infrastructure/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Data;
using OnlineStore.Infrastructure.Repositories;

namespace OnlineStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

            return services;
        }
    }
}
```

## 5. Application Layer Implementation

### DTOs (Data Transfer Objects)

#### UserDto.cs

```csharp
// OnlineStore.Application/DTOs/UserDto.cs
using System;

namespace OnlineStore.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

#### ProductDto.cs

```csharp
// OnlineStore.Application/DTOs/ProductDto.cs
using System;

namespace OnlineStore.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
```

#### CategoryDto.cs

```csharp
// OnlineStore.Application/DTOs/CategoryDto.cs
using System;

namespace OnlineStore.Application.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
    }
}
```

#### OrderDto.cs

```csharp
// OnlineStore.Application/DTOs/OrderDto.cs
using System;
using System.Collections.Generic;

namespace OnlineStore.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
```

#### OrderItemDto.cs

```csharp
// OnlineStore.Application/DTOs/OrderItemDto.cs
using System;

namespace OnlineStore.Application.DTOs
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
```

#### ShoppingCartDto.cs

```csharp
// OnlineStore.Application/DTOs/ShoppingCartDto.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineStore.Application.DTOs
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
        public int TotalItems => Items.Sum(i => i.Quantity);
    }
}
```

#### CartItemDto.cs

```csharp
// OnlineStore.Application/DTOs/CartItemDto.cs
using System;

namespace OnlineStore.Application.DTOs
{
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
```

### AutoMapper Profile

```csharp
// OnlineStore.Application/Mappings/MappingProfile.cs
using AutoMapper;
using OnlineStore.Application.DTOs;
using OnlineStore.Core.Entities;

namespace OnlineStore.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            
            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
            
            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null));
            
            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            
            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
            
            // ShoppingCart mappings
            CreateMap<ShoppingCart, ShoppingCartDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));
            
            // CartItem mappings
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : null))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0));
        }
    }
}
```

### Services

#### IProductService.cs

```csharp
// OnlineStore.Application/Services/Interfaces/IProductService.cs
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
```

#### ProductService.cs

```csharp
// OnlineStore.Application/Services/ProductService.cs
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
```

#### IOrderService.cs

```csharp
// OnlineStore.Application/Services/Interfaces/IOrderService.cs
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
```

#### OrderService.cs

```csharp
// OnlineStore.Application/Services/OrderService.cs
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
```

#### IShoppingCartService.cs

```csharp
// OnlineStore.Application/Services/Interfaces/IShoppingCartService.cs
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
```

#### ShoppingCartService.cs

```csharp
// OnlineStore.Application/Services/ShoppingCartService.cs
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
                // Create a new cart if it doesn't exist
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _cartRepository.AddAsync(cart);
            }

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task<ShoppingCartDto> AddItemToCartAsync(Guid userId, Guid productId, int quantity)
        {
            // Check if product exists and is in stock
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
            {
                throw new Exception("Product not found or not available.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new Exception("Not enough stock available.");
            }

            // Get or create cart
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _cartRepository.AddAsync(cart);
            }

            // Check if item already exists in cart
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem != null)
            {
                // Update quantity
                cartItem.Quantity += quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(cartItem);
            }
            else
            {
                // Add new item
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _cartRepository.AddAsync(cartItem);
            }

            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);

            // Return updated cart
            return await GetCartByUserIdAsync(userId);
        }

        public async Task<ShoppingCartDto> UpdateCartItemQuantityAsync(Guid userId, Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                return await RemoveItemFromCartAsync(userId, productId);
            }

            // Check if product exists and is in stock
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || !product.IsActive)
            {
                throw new Exception("Product not found or not available.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new Exception("Not enough stock available.");
            }

            // Get cart
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                throw new Exception("Shopping cart not found.");
            }

            // Update item quantity
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
            {
                throw new Exception("Item not found in cart.");
            }

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cartItem);

            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);

            // Return updated cart
            return await GetCartByUserIdAsync(userId);
        }

        public async Task<ShoppingCartDto> RemoveItemFromCartAsync(Guid userId, Guid productId)
        {
            // Get cart
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                throw new Exception("Shopping cart not found.");
            }

            // Remove item
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem != null)
            {
                await _cartRepository.DeleteAsync(cartItem);

                // Update cart timestamp
                cart.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(cart);
            }

            // Return updated cart
            return await GetCartByUserIdAsync(userId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            // Get cart
            var cart = await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                return;
            }

            // Remove all items
            foreach (var item in cart.CartItems)
            {
                await _cartRepository.DeleteAsync(item);
            }

            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);
        }
    }
}
```

### Application Service Registration

```csharp
// OnlineStore.Application/DependencyInjection.cs
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Application.Mappings;
using OnlineStore.Application.Services;
using OnlineStore.Application.Services.Interfaces;
using System.Reflection;

namespace OnlineStore.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Register services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();

            return services;
        }
    }
}
```

## 6. API Layer Implementation

### Controllers

#### ProductsController.cs

```csharp
// OnlineStore.API/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<ProductSearchResult>> SearchProducts(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _productService.SearchProductsAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ProductSearchResult>> GetProductsByCategory(
            Guid categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
        {
            var result = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            try
            {
                var result = await _productService.UpdateProductAsync(productDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
```

#### OrdersController.cs

```csharp
// OnlineStore.API/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Check if user is authorized to view this order
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (order.UserId != userId && userRole != "Manager" && userRole != "Admin")
            {
                return Forbid();
            }

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetUserOrders()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersByStatus(
            string status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status, pageNumber, pageSize);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var order = await _orderService.CreateOrderAsync(userId, request.ShippingAddress, request.BillingAddress);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(Guid id, UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, request.Status);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

    public class CreateOrderRequest
    {
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; }
    }
}
```

#### ShoppingCartController.cs

```csharp
// OnlineStore.API/Controllers/ShoppingCartController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _cartService;

        public ShoppingCartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<ShoppingCartDto>> GetCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<ShoppingCartDto>> AddItemToCart(AddCartItemRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var cart = await _cartService.AddItemToCartAsync(userId, request.ProductId, request.Quantity);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("items/{productId}")]
        public async Task<ActionResult<ShoppingCartDto>> UpdateCartItemQuantity(Guid productId, UpdateCartItemRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var cart = await _cartService.UpdateCartItemQuantityAsync(userId, productId, request.Quantity);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<ShoppingCartDto>> RemoveItemFromCart(Guid productId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var cart = await _cartService.RemoveItemFromCartAsync(userId, productId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> ClearCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }

    public class AddCartItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }
}
```

### Program.cs

```csharp
// OnlineStore.API/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineStore.Application;
using OnlineStore.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OnlineStore API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add Application and Infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### appsettings.json

```json
// OnlineStore.API/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=onlinestore;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your_secret_key_at_least_16_characters_long",
    "Issuer": "OnlineStore",
    "Audience": "OnlineStoreClients",
    "DurationInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 7. Unit Testing

### Product Service Tests

```csharp
// OnlineStore.UnitTests/Services/ProductServiceTests.cs
using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Mappings;
using OnlineStore.Application.Services;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace OnlineStore.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly IMapper _mapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            
            // Configure AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
            
            _productService = new ProductService(_mockProductRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                StockQuantity = 10,
                IsActive = true
            };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Test Product");
            result.Price.Should().Be(99.99m);
        }

        [Fact]
        public async Task SearchProductsAsync_ShouldReturnProducts_WhenSearchTermMatches()
        {
            // Arrange
            var searchTerm = "test";
            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Product 1",
                    Description = "Test Description 1",
                    Price = 99.99m,
                    StockQuantity = 10,
                    IsActive = true
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Product 2",
                    Description = "Test Description 2",
                    Price = 149.99m,
                    StockQuantity = 5,
                    IsActive = true
                }
            };

            _mockProductRepository.Setup(repo => repo.SearchProductsAsync(searchTerm, 1, 10))
                .ReturnsAsync(products);
            _mockProductRepository.Setup(repo => repo.GetTotalProductCountAsync(searchTerm))
                .ReturnsAsync(2);

            // Act
            var result = await _productService.SearchProductsAsync(searchTerm, 1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldReturnCreatedProduct()
        {
            // Arrange
            var productDto = new ProductDto
            {
                Name = "New Product",
                Description = "New Description",
                Price = 199.99m,
                StockQuantity = 20,
                IsActive = true
            };

            _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product product) => product);

            // Act
            var result = await _productService.CreateProductAsync(productDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New Product");
            result.Price.Should().Be(199.99m);
            
            _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldReturnUpdatedProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Existing Product",
                Description = "Existing Description",
                Price = 99.99m,
                StockQuantity = 10,
                IsActive = true
            };

            var updatedProductDto = new ProductDto
            {
                Id = productId,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 149.99m,
                StockQuantity = 15,
                IsActive = true
            };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);
            _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.UpdateProductAsync(updatedProductDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated Product");
            result.Price.Should().Be(149.99m);
            
            _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldDeleteProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Existing Product",
                Description = "Existing Description",
                Price = 99.99m,
                StockQuantity = 10,
                IsActive = true
            };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);
            _mockProductRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            // Act
            await _productService.DeleteProductAsync(productId);

            // Assert
            _mockProductRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task IsProductInStockAsync_ShouldReturnTrue_WhenProductHasSufficientStock()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                StockQuantity = 10,
                IsActive = true
            };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.IsProductInStockAsync(productId, 5);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsProductInStockAsync_ShouldReturnFalse_WhenProductHasInsufficientStock()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                StockQuantity = 10,
                IsActive = true
            };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.IsProductInStockAsync(productId, 15);

            // Assert
            result.Should().BeFalse();
        }
    }
}
```

### Order Service Tests

```csharp
// OnlineStore.UnitTests/Services/OrderServiceTests.cs
using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineStore.Application.Mappings;
using OnlineStore.Application.Services;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OnlineStore.UnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IShoppingCartRepository> _mockCartRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly IMapper _mapper;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockCartRepository = new Mock<IShoppingCartRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            
            // Configure AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
            
            _orderService = new OrderService(
                _mockOrderRepository.Object,
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mapper);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                UserId = userId,
                Status = "Pending",
                TotalAmount = 99.99m,
                ShippingAddress = "123 Test St",
                BillingAddress = "123 Test St",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            _mockOrderRepository.Setup(repo => repo.GetOrderWithItemsAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(orderId);
            result.UserId.Should().Be(userId);
            result.Status.Should().Be("Pending");
            result.TotalAmount.Should().Be(99.99m);
        }

        [Fact]
        public async Task GetOrdersByUserIdAsync_ShouldReturnUserOrders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Status = "Pending",
                    TotalAmount = 99.99m,
                    ShippingAddress = "123 Test St",
                    BillingAddress = "123 Test St",
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Status = "Shipped",
                    TotalAmount = 149.99m,
                    ShippingAddress = "123 Test St",
                    BillingAddress = "123 Test St",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    OrderItems = new List<OrderItem>()
                }
            };

            _mockOrderRepository.Setup(repo => repo.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetOrdersByUserIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrder_WhenCartHasItems()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            
            var cart = new ShoppingCart
            {
                Id = cartId,
                UserId = userId,
                CartItems = new List<CartItem>
                {
                    new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = 2
                    }
                }
            };
            
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 49.99m,
                StockQuantity = 10,
                IsActive = true
            };

            _mockCartRepository.Setup(repo => repo.GetCartWithItemsByUserIdAsync(userId))
                .ReturnsAsync(cart);
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);
            _mockOrderRepository.Setup(repo => repo.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order order) => order);
            _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);
            _mockCartRepository.Setup(repo => repo.DeleteAsync(It.IsAny<CartItem>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.CreateOrderAsync(userId, "123 Test St", "123 Test St");

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.Status.Should().Be("Pending");
            result.TotalAmount.Should().Be(99.98m); // 2 * 49.99
            result.ShippingAddress.Should().Be("123 Test St");
            result.BillingAddress.Should().Be("123 Test St");
            
            _mockOrderRepository.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Once);
            _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
            _mockCartRepository.Verify(repo => repo.DeleteAsync(It.IsAny<CartItem>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldUpdateStatus_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            _mockOrderRepository.Setup(repo => repo.GetOrderWithItemsAsync(orderId))
                .ReturnsAsync(order);
            _mockOrderRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(orderId, "Shipped");

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Shipped");
            
            _mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Once);
        }
    }
}
```

## 8. API Configuration and Startup

### Configuring Authentication and Authorization

```csharp
// OnlineStore.API/Services/AuthService.cs
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.API.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
```

### Authentication Controller

```csharp
// OnlineStore.API/Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.API.Services;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;

        public AuthController(IUserRepository userRepository, AuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            if (!BC.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _authService.GenerateJwtToken(user);

            return Ok(new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = token
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Check if email already exists
            existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BC.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = "User", // Default role for new registrations
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            var token = _authService.GenerateJwtToken(user);

            return Ok(new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = token
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<AuthResponse>> GetCurrentUser()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = null // No need to generate a new token
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AuthResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
```

### User Management Controller (Admin Only)

```csharp
// OnlineStore.API/Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Core.Entities;
using OnlineStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace OnlineStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string role)
        {
            if (role != "User" && role != "Manager" && role != "Admin")
            {
                return BadRequest("Invalid role specified");
            }

            var users = await _userRepository.GetUsersByRoleAsync(role);
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Check if email already exists
            existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Validate role
            if (request.Role != "User" && request.Role != "Manager" && request.Role != "Admin")
            {
                return BadRequest(new { message = "Invalid role specified" });
            }

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BC.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, _mapper.Map<UserDto>(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if username is being changed and already exists
            if (request.Username != user.Username)
            {
                var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username already exists" });
                }
            }

            // Check if email is being changed and already exists
            if (request.Email != user.Email)
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email already exists" });
                }
            }

            // Validate role
            if (request.Role != "User" && request.Role != "Manager" && request.Role != "Admin")
            {
                return BadRequest(new { message = "Invalid role specified" });
            }

            // Update user
            user.Username = request.Username;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            // Update password if provided
            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = BC.HashPassword(request.Password);
            }

            await _userRepository.UpdateAsync(user);
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteAsync(user);
            return NoContent();
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Optional
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}
```

This completes the backend development for the online store application. The next section will cover the React frontend development.
