

```sql

-- Connect to PostgreSQL as postgres user
psql -U postgres
  
-- Create a new database
CREATE DATABASE onlinestore;

-- Connect to the new database
\c onlinestore

-- Create extension for UUID generation
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create extension for full-text search
CREATE EXTENSION IF NOT EXISTS pg_trgm;

```

  

## Database Schema
Below is the complete database schema for the online store application:

```sql
-- Users table with role-based access control
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    role VARCHAR(20) NOT NULL CHECK (role IN ('User', 'Manager', 'Admin')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Categories table
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    description TEXT,
    parent_id UUID REFERENCES categories(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Products table with tsvector for search
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL,
    stock_quantity INTEGER NOT NULL DEFAULT 0,
    category_id UUID REFERENCES categories(id),
    image_url VARCHAR(255),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    search_vector TSVECTOR
);

-- Create a trigger function to update the search vector
CREATE OR REPLACE FUNCTION products_search_vector_update() RETURNS TRIGGER AS $$
BEGIN
    NEW.search_vector =
        setweight(to_tsvector('english', COALESCE(NEW.name, '')), 'A') ||
        setweight(to_tsvector('english', COALESCE(NEW.description, '')), 'B');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create a trigger to update the search vector on insert or update
CREATE TRIGGER products_search_vector_update
BEFORE INSERT OR UPDATE ON products
FOR EACH ROW
EXECUTE FUNCTION products_search_vector_update();

-- Create a GIN index for the search vector
CREATE INDEX products_search_idx ON products USING GIN(search_vector);

-- Orders table
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    status VARCHAR(20) NOT NULL CHECK (status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
    total_amount DECIMAL(10, 2) NOT NULL,
    shipping_address TEXT NOT NULL,
    billing_address TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Order items table
CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    order_id UUID REFERENCES orders(id),
    product_id UUID REFERENCES products(id),
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Shopping cart table
CREATE TABLE shopping_carts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Cart items table
CREATE TABLE cart_items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    cart_id UUID REFERENCES shopping_carts(id),
    product_id UUID REFERENCES products(id),
    quantity INTEGER NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Product reviews table
CREATE TABLE product_reviews (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    product_id UUID REFERENCES products(id),
    user_id UUID REFERENCES users(id),
    rating INTEGER NOT NULL CHECK (rating BETWEEN 1 AND 5),
    comment TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for frequently queried columns
CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_orders_user ON orders(user_id);
CREATE INDEX idx_order_items_order ON order_items(order_id);
CREATE INDEX idx_cart_items_cart ON cart_items(cart_id);
CREATE INDEX idx_product_reviews_product ON product_reviews(product_id);
```

  

##  Using tsvector for Enhanced Search
The schema above includes a `search_vector` column in the `products` table, which is automatically updated using a trigger whenever a product is inserted or updated. This allows for powerful full-text search capabilities.

```sql

-- Basic search for products containing 'phone'

SELECT id, name, description, price
FROM products
WHERE search_vector @@ to_tsquery('english', 'phone');

-- Ranked search results
SELECT id, name, description, price,
       ts_rank(search_vector, to_tsquery('english', 'smartphone')) AS rank
FROM products
WHERE search_vector @@ to_tsquery('english', 'smartphone')
ORDER BY rank DESC;

-- Search with multiple terms (AND condition)
SELECT id, name, description, price
FROM products
WHERE search_vector @@ to_tsquery('english', 'wireless & headphones');

-- Search with multiple terms (OR condition)
SELECT id, name, description, price
FROM products
WHERE search_vector @@ to_tsquery('english', 'laptop | notebook');

-- Prefix search
SELECT id, name, description, price
FROM products
WHERE search_vector @@ to_tsquery('english', 'smart:*');
```

## 5. Database Initialization Script
Create a file named `init_database.sql` with all the SQL commands above to initialize the database:


```sql
-- Database initialization script for Online Store Application
-- Run this script to create all necessary tables and indexes
-- Create extensions

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- Create tables
-- Users table

CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    role VARCHAR(20) NOT NULL CHECK (role IN ('User', 'Manager', 'Admin')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Categories table
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    description TEXT,
    parent_id UUID REFERENCES categories(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Products table with tsvector for search
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL,
    stock_quantity INTEGER NOT NULL DEFAULT 0,
    category_id UUID REFERENCES categories(id),
    image_url VARCHAR(255),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    search_vector TSVECTOR
);

-- Create a trigger function to update the search vector
CREATE OR REPLACE FUNCTION products_search_vector_update() RETURNS TRIGGER AS $$
BEGIN
    NEW.search_vector =
        setweight(to_tsvector('english', COALESCE(NEW.name, '')), 'A') ||
        setweight(to_tsvector('english', COALESCE(NEW.description, '')), 'B');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create a trigger to update the search vector on insert or update
CREATE TRIGGER products_search_vector_update
BEFORE INSERT OR UPDATE ON products
FOR EACH ROW
EXECUTE FUNCTION products_search_vector_update();

-- Create a GIN index for the search vector
CREATE INDEX products_search_idx ON products USING GIN(search_vector);

-- Orders table
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    status VARCHAR(20) NOT NULL CHECK (status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
    total_amount DECIMAL(10, 2) NOT NULL,
    shipping_address TEXT NOT NULL,
    billing_address TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Order items table
CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    order_id UUID REFERENCES orders(id),
    product_id UUID REFERENCES products(id),
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

  
-- Shopping cart table
CREATE TABLE shopping_carts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Cart items table
CREATE TABLE cart_items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    cart_id UUID REFERENCES shopping_carts(id),
    product_id UUID REFERENCES products(id),
    quantity INTEGER NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

  
-- Product reviews table
CREATE TABLE product_reviews (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    product_id UUID REFERENCES products(id),
    user_id UUID REFERENCES users(id),
    rating INTEGER NOT NULL CHECK (rating BETWEEN 1 AND 5),
    comment TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

  

-- Create indexes for frequently queried columns

CREATE INDEX idx_products_category ON products(category_id);

CREATE INDEX idx_orders_user ON orders(user_id);

CREATE INDEX idx_order_items_order ON order_items(order_id);

CREATE INDEX idx_cart_items_cart ON cart_items(cart_id);

CREATE INDEX idx_product_reviews_product ON product_reviews(product_id);

  

-- Insert sample data for testing

-- Insert admin user (password: Admin123!)

INSERT INTO users (username, email, password_hash, first_name, last_name, role)

VALUES ('admin', 'admin@example.com', '$2a$11$ov8a4UUYkIgbZM/ivITx7uNZxSEgb8HFQdwJnlTaJ8QA3NMI2Txe2', 'Admin', 'User', 'Admin');

  

-- Insert manager user (password: Manager123!)

INSERT INTO users (username, email, password_hash, first_name, last_name, role)

VALUES ('manager', 'manager@example.com', '$2a$11$9XxjHbmxJ1VFv5zFOKlEW.pOKGaS9a6F/gNiJxXwAAZEqfXbLyOQK', 'Manager', 'User', 'Manager');

  

-- Insert regular user (password: User123!)

INSERT INTO users (username, email, password_hash, first_name, last_name, role)

VALUES ('user', 'user@example.com', '$2a$11$5giZZJfL8ZJ1zGPEEWjIUeNRlX0obnwS/4kII0YidB3IFKyz2Eif2', 'Regular', 'User', 'User');

  

-- Insert categories

INSERT INTO categories (name, description)

VALUES ('Electronics', 'Electronic devices and accessories');

  

INSERT INTO categories (name, description)

VALUES ('Clothing', 'Apparel and fashion items');

  

-- Get category IDs

DO $$

DECLARE

    electronics_id UUID;

    clothing_id UUID;

BEGIN

    SELECT id INTO electronics_id FROM categories WHERE name = 'Electronics';

    SELECT id INTO clothing_id FROM categories WHERE name = 'Clothing';

    -- Insert products

    INSERT INTO products (name, description, price, stock_quantity, category_id, is_active)

    VALUES ('Smartphone X', 'Latest smartphone with advanced features and high-resolution camera', 699.99, 50, electronics_id, TRUE);

    INSERT INTO products (name, description, price, stock_quantity, category_id, is_active)

    VALUES ('Wireless Headphones', 'Noise-cancelling wireless headphones with long battery life', 149.99, 100, electronics_id, TRUE);

    INSERT INTO products (name, description, price, stock_quantity, category_id, is_active)

    VALUES ('Laptop Pro', 'Powerful laptop for professionals with high performance', 1299.99, 25, electronics_id, TRUE);

    INSERT INTO products (name, description, price, stock_quantity, category_id, is_active)

    VALUES ('T-shirt Basic', 'Comfortable cotton t-shirt for everyday wear', 19.99, 200, clothing_id, TRUE);

    INSERT INTO products (name, description, price, stock_quantity, category_id, is_active)

    VALUES ('Jeans Classic', 'Classic fit jeans with durable denim material', 49.99, 150, clothing_id, TRUE);

END $$;

```

  

## 6. Running the Database Initialization Script

  

To initialize the database with the schema and sample data:

  

```bash

# Create the database

sudo -u postgres createdb onlinestore

  

# Run the initialization script

sudo -u postgres psql -d onlinestore -f init_database.sql

```

  

## 7. Database Connection String for .NET Application

  

In your .NET application, use the following connection string format:

  

```

"ConnectionStrings": {

  "DefaultConnection": "Host=localhost;Database=onlinestore;Username=postgres;Password=your_password"

}

```

  

Replace `your_password` with the actual password for your PostgreSQL user.

  

## 8. Entity Framework Core Configuration

  

When using Entity Framework Core with PostgreSQL, install the required NuGet packages:

  

```bash

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

dotnet add package Microsoft.EntityFrameworkCore.Design

```

  

This completes the PostgreSQL database setup with tsvector for enhanced search capabilities. The next section will cover the .NET Web API backend development.