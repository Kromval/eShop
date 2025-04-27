using System;
using System.Collections.Generic;

namespace OnlineStore.Core.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public required Category ParentCategory { get; set; }
        public ICollection<Category> Subcategories { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}