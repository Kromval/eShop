# Step-by-Step Guide to Develop React Frontend for Online Store Application

  

This guide provides detailed instructions for developing a React frontend that integrates with the .NET Web API backend for an online store application.

  

## Table of Contents

  

1. [Project Setup](#1-project-setup)

2. [Project Structure](#2-project-structure)

3. [Environment Configuration](#3-environment-configuration)

4. [Component Hierarchy](#4-component-hierarchy)

5. [Core Components Implementation](#5-core-components-implementation)

6. [API Integration](#6-api-integration)

7. [State Management](#7-state-management)

8. [Styling and Responsive Design](#8-styling-and-responsive-design)

9. [Testing](#9-testing)

10. [Deployment](#10-deployment)

  

## 1. Project Setup

  

### Prerequisites

  

- Node.js (v16.0.0 or later)

- npm (v8.0.0 or later) or yarn (v1.22.0 or later)

- Code editor (VS Code recommended)

- Git for version control

  

### Creating a New React Project

  

```bash

# Create a new React project using Create React App

npx create-react-app online-store-frontend

cd online-store-frontend

  

# OR using TypeScript template (recommended for type safety)

npx create-react-app online-store-frontend --template typescript

cd online-store-frontend

```

  

### Installing Essential Dependencies

  

```bash

# Install routing

npm install react-router-dom

  

# Install HTTP client for API calls

npm install axios

  

# Install UI component library

npm install @mui/material @emotion/react @emotion/styled

  

# Install form handling

npm install react-hook-form

  

# Install state management

npm install @reduxjs/toolkit react-redux

  

# Install for handling authentication

npm install jwt-decode

  

# Install for date formatting

npm install date-fns

```

  

## 2. Project Structure

  

Create a well-organized folder structure for your React application:

  

```

online-store-frontend/

├── public/

│   ├── index.html

│   ├── favicon.ico

│   └── ...

├── src/

│   ├── assets/           # Static assets like images, fonts, etc.

│   │   ├── images/

│   │   └── styles/

│   ├── components/       # Reusable UI components

│   │   ├── common/       # Shared components like Button, Card, etc.

│   │   ├── layout/       # Layout components like Header, Footer, etc.

│   │   └── features/     # Feature-specific components

│   ├── hooks/            # Custom React hooks

│   ├── pages/            # Page components for routing

│   │   ├── Home/

│   │   ├── Products/

│   │   ├── ProductDetail/

│   │   ├── Cart/

│   │   ├── Checkout/

│   │   ├── Orders/

│   │   ├── Profile/

│   │   ├── Auth/

│   │   └── Admin/

│   ├── services/         # API service functions

│   │   ├── api.js        # Base API configuration

│   │   ├── authService.js

│   │   ├── productService.js

│   │   ├── cartService.js

│   │   ├── orderService.js

│   │   └── userService.js

│   ├── store/            # Redux store configuration

│   │   ├── index.js

│   │   └── slices/       # Redux slices for different features

│   │       ├── authSlice.js

│   │       ├── productSlice.js

│   │       ├── cartSlice.js

│   │       └── orderSlice.js

│   ├── utils/            # Utility functions

│   │   ├── formatters.js

│   │   ├── validators.js

│   │   └── helpers.js

│   ├── constants/        # Application constants

│   │   ├── apiConstants.js

│   │   └── uiConstants.js

│   ├── types/            # TypeScript type definitions (if using TS)

│   ├── App.js            # Main App component

│   ├── index.js          # Entry point

│   └── routes.js         # Application routes

├── .env                  # Environment variables

├── .env.development      # Development environment variables

├── .env.production       # Production environment variables

├── package.json

└── README.md

```

  

### Creating the Basic Folder Structure

  

```bash

# Create main directories

mkdir -p src/assets/images src/assets/styles

mkdir -p src/components/common src/components/layout src/components/features

mkdir -p src/hooks

mkdir -p src/pages/Home src/pages/Products src/pages/ProductDetail src/pages/Cart src/pages/Checkout src/pages/Orders src/pages/Profile src/pages/Auth src/pages/Admin

mkdir -p src/services

mkdir -p src/store/slices

mkdir -p src/utils

mkdir -p src/constants

mkdir -p src/types

```

  

### Setting Up Base Files

  

Create the following base files to establish the project structure:

  

#### src/services/api.js

  

```javascript

import axios from 'axios';

  

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

  

const api = axios.create({

  baseURL: API_URL,

  headers: {

    'Content-Type': 'application/json',

  },

});

  

// Add a request interceptor for authentication

api.interceptors.request.use(

  (config) => {

    const token = localStorage.getItem('token');

    if (token) {

      config.headers['Authorization'] = `Bearer ${token}`;

    }

    return config;

  },

  (error) => {

    return Promise.reject(error);

  }

);

  

// Add a response interceptor for error handling

api.interceptors.response.use(

  (response) => {

    return response;

  },

  (error) => {

    // Handle 401 Unauthorized errors (token expired)

    if (error.response && error.response.status === 401) {

      localStorage.removeItem('token');

      window.location.href = '/login';

    }

    return Promise.reject(error);

  }

);

  

export default api;

```

  

#### src/routes.js

  

```javascript

import { lazy, Suspense } from 'react';

import { Navigate } from 'react-router-dom';

  

// Lazy load page components

const Home = lazy(() => import('./pages/Home/Home'));

const ProductList = lazy(() => import('./pages/Products/ProductList'));

const ProductDetail = lazy(() => import('./pages/ProductDetail/ProductDetail'));

const Cart = lazy(() => import('./pages/Cart/Cart'));

const Checkout = lazy(() => import('./pages/Checkout/Checkout'));

const OrderList = lazy(() => import('./pages/Orders/OrderList'));

const OrderDetail = lazy(() => import('./pages/Orders/OrderDetail'));

const Profile = lazy(() => import('./pages/Profile/Profile'));

const Login = lazy(() => import('./pages/Auth/Login'));

const Register = lazy(() => import('./pages/Auth/Register'));

const AdminDashboard = lazy(() => import('./pages/Admin/Dashboard'));

const AdminProducts = lazy(() => import('./pages/Admin/Products'));

const AdminOrders = lazy(() => import('./pages/Admin/Orders'));

const AdminUsers = lazy(() => import('./pages/Admin/Users'));

const NotFound = lazy(() => import('./pages/NotFound'));

  

// Loading component for suspense fallback

const Loading = () => <div className="loading">Loading...</div>;

  

// Protected route component

const ProtectedRoute = ({ children, requiredRole = 'User' }) => {

  const isAuthenticated = localStorage.getItem('token') !== null;

  const userRole = localStorage.getItem('userRole') || 'Guest';

  if (!isAuthenticated) {

    return <Navigate to="/login" replace />;

  }

  if (requiredRole !== 'User' && userRole !== requiredRole) {

    return <Navigate to="/" replace />;

  }

  return children;

};

  

const routes = [

  {

    path: '/',

    element: (

      <Suspense fallback={<Loading />}>

        <Home />

      </Suspense>

    ),

  },

  {

    path: '/products',

    element: (

      <Suspense fallback={<Loading />}>

        <ProductList />

      </Suspense>

    ),

  },

  {

    path: '/products/:id',

    element: (

      <Suspense fallback={<Loading />}>

        <ProductDetail />

      </Suspense>

    ),

  },

  {

    path: '/cart',

    element: (

      <Suspense fallback={<Loading />}>

        <Cart />

      </Suspense>

    ),

  },

  {

    path: '/checkout',

    element: (

      <ProtectedRoute>

        <Suspense fallback={<Loading />}>

          <Checkout />

        </Suspense>

      </ProtectedRoute>

    ),

  },

  {

    path: '/orders',

    element: (

      <ProtectedRoute>

        <Suspense fallback={<Loading />}>

          <OrderList />

        </Suspense>

      </ProtectedRoute>

    ),

  },

  {

    path: '/orders/:id',

    element: (

      <ProtectedRoute>

        <Suspense fallback={<Loading />}>

          <OrderDetail />

        </Suspense>

      </ProtectedRoute>

    ),

  },

  {

    path: '/profile',

    element: (

      <ProtectedRoute>

        <Suspense fallback={<Loading />}>

          <Profile />

        </Suspense>

      </ProtectedRoute>

    ),

  },

  {

    path: '/login',

    element: (

      <Suspense fallback={<Loading />}>

        <Login />

      </Suspense>

    ),

  },

  {

    path: '/register',

    element: (

      <Suspense fallback={<Loading />}>

        <Register />

      </Suspense>

    ),

  },

  {

    path: '/admin',

    element: (

      <ProtectedRoute requiredRole="Admin">

        <Suspense fallback={<Loading />}>

          <AdminDashboard />

        </Suspense>

      </ProtectedRoute>

    ),

  },

  {

    path: '/admin/products',

    element: (

      <ProtectedRoute requiredRole="Admin">

        <Suspense fallback={<Loading />}>

          <AdminProducts />

        </Suspense>

      </ProtectedRoute>

    ),

  },

  {

    path: '/admin/orders',

    element: (

      <ProtectedRoute requiredRole="Admin">

        <Suspense fallback={<Loading />}>

          <AdminOrders />

        </Suspense>

      </ProtectedRoute>

    ),

  },

  {

    path: '/admin/users',

    element: (

      <ProtectedRoute requiredRole="Admin">

        <Suspense fallback={<Loading />}>

          <AdminUsers />

        </Suspense>

      </ProtectedRoute>

    ),

  },

  {

    path: '*',

    element: (

      <Suspense fallback={<Loading />}>

        <NotFound />

      </Suspense>

    ),

  },

];

  

export default routes;

```

  

#### src/App.js

  

```javascript

import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import { Provider } from 'react-redux';

import { ThemeProvider, createTheme } from '@mui/material/styles';

import CssBaseline from '@mui/material/CssBaseline';

import routes from './routes';

import store from './store';

import Header from './components/layout/Header';

import Footer from './components/layout/Footer';

  

// Create a theme instance

const theme = createTheme({

  palette: {

    primary: {

      main: '#1976d2',

    },

    secondary: {

      main: '#dc004e',

    },

  },

});

  

function App() {

  return (

    <Provider store={store}>

      <ThemeProvider theme={theme}>

        <CssBaseline />

        <Router>

          <div className="app">

            <Header />

            <main className="main-content">

              <Routes>

                {routes.map((route, index) => (

                  <Route key={index} path={route.path} element={route.element} />

                ))}

              </Routes>

            </main>

            <Footer />

          </div>

        </Router>

      </ThemeProvider>

    </Provider>

  );

}

  

export default App;

```

  

#### src/store/index.js

  

```javascript

import { configureStore } from '@reduxjs/toolkit';

import authReducer from './slices/authSlice';

import productReducer from './slices/productSlice';

import cartReducer from './slices/cartSlice';

import orderReducer from './slices/orderSlice';

  

const store = configureStore({

  reducer: {

    auth: authReducer,

    products: productReducer,

    cart: cartReducer,

    orders: orderReducer,

  },

});

  

export default store;

```

  

#### .env.development

  

```

REACT_APP_API_URL=http://localhost:5000/api

```

  

#### .env.production

  

```

REACT_APP_API_URL=/api

```

  

## 3. Environment Configuration

  

Setting up the proper environment configuration is crucial for a React application to work correctly across different environments (development, testing, production).

  

### Environment Variables

  

Create React App provides built-in support for environment variables. These variables are embedded during the build time and can be accessed via `process.env.REACT_APP_*`.

  

#### Environment Files

  

Create the following environment files at the root of your project:

  

1. `.env`: Default environment variables for all environments

2. `.env.development`: Variables for development environment (overrides `.env`)

3. `.env.production`: Variables for production environment (overrides `.env`)

4. `.env.test`: Variables for test environment (overrides `.env`)

  

Example of environment variables for the online store application:

  

```

# .env - Default environment variables

REACT_APP_VERSION=$npm_package_version

REACT_APP_NAME=Online Store

  

# .env.development - Development environment variables

REACT_APP_API_URL=http://localhost:5000/api

REACT_APP_ENV=development

REACT_APP_DEBUG=true

  

# .env.production - Production environment variables

REACT_APP_API_URL=/api

REACT_APP_ENV=production

REACT_APP_DEBUG=false

  

# .env.test - Test environment variables

REACT_APP_API_URL=http://localhost:5000/api

REACT_APP_ENV=test

REACT_APP_DEBUG=true

```

  

### Development Server Configuration

  

Create React App provides a development server with hot-reloading capabilities. You can customize its behavior by modifying the `package.json` file:

  

```json

{

  "scripts": {

    "start": "react-scripts start",

    "build": "react-scripts build",

    "test": "react-scripts test",

    "eject": "react-scripts eject",

    "start:dev": "env-cmd -f .env.development react-scripts start",

    "build:prod": "env-cmd -f .env.production react-scripts build"

  }

}

```

  

For more advanced configuration, you can create a `setupProxy.js` file in the `src` directory to configure the development server proxy:

  

#### src/setupProxy.js

  

```javascript

const { createProxyMiddleware } = require('http-proxy-middleware');

  

module.exports = function(app) {

  app.use(

    '/api',

    createProxyMiddleware({

      target: 'http://localhost:5000',

      changeOrigin: true,

    })

  );

};

```

  

### Build Configuration

  

Create React App handles the build process automatically. The build output is optimized for production by default, including minification, bundling, and code splitting.

  

To customize the build process, you can:

  

1. Use environment variables to control build behavior

2. Add a `craco.config.js` file for more advanced configuration without ejecting

  

#### Installing CRACO (Create React App Configuration Override)

  

```bash

npm install @craco/craco

```

  

#### craco.config.js

  

```javascript

const path = require('path');

  

module.exports = {

  webpack: {

    alias: {

      '@': path.resolve(__dirname, 'src'),

      '@components': path.resolve(__dirname, 'src/components'),

      '@pages': path.resolve(__dirname, 'src/pages'),

      '@services': path.resolve(__dirname, 'src/services'),

      '@store': path.resolve(__dirname, 'src/store'),

      '@utils': path.resolve(__dirname, 'src/utils'),

      '@assets': path.resolve(__dirname, 'src/assets'),

    },

  },

  jest: {

    configure: {

      moduleNameMapper: {

        '^@/(.*)$': '<rootDir>/src/$1',

        '^@components/(.*)$': '<rootDir>/src/components/$1',

        '^@pages/(.*)$': '<rootDir>/src/pages/$1',

        '^@services/(.*)$': '<rootDir>/src/services/$1',

        '^@store/(.*)$': '<rootDir>/src/store/$1',

        '^@utils/(.*)$': '<rootDir>/src/utils/$1',

        '^@assets/(.*)$': '<rootDir>/src/assets/$1',

      },

    },

  },

};

```

  

Update your `package.json` scripts to use CRACO:

  

```json

{

  "scripts": {

    "start": "craco start",

    "build": "craco build",

    "test": "craco test",

    "eject": "react-scripts eject"

  }

}

```

  

### TypeScript Configuration (if using TypeScript)

  

If you're using TypeScript, you'll need to configure it properly for your project. Create or update the `tsconfig.json` file:

  

```json

{

  "compilerOptions": {

    "target": "es5",

    "lib": ["dom", "dom.iterable", "esnext"],

    "allowJs": true,

    "skipLibCheck": true,

    "esModuleInterop": true,

    "allowSyntheticDefaultImports": true,

    "strict": true,

    "forceConsistentCasingInFileNames": true,

    "noFallthroughCasesInSwitch": true,

    "module": "esnext",

    "moduleResolution": "node",

    "resolveJsonModule": true,

    "isolatedModules": true,

    "noEmit": true,

    "jsx": "react-jsx",

    "baseUrl": "src",

    "paths": {

      "@/*": ["./*"],

      "@components/*": ["components/*"],

      "@pages/*": ["pages/*"],

      "@services/*": ["services/*"],

      "@store/*": ["store/*"],

      "@utils/*": ["utils/*"],

      "@assets/*": ["assets/*"]

    }

  },

  "include": ["src"]

}

```

  

### Browser Support Configuration

  

To specify which browsers your application should support, update the `browserslist` section in your `package.json`:

  

```json

{

  "browserslist": {

    "production": [

      ">0.2%",

      "not dead",

      "not op_mini all"

    ],

    "development": [

      "last 1 chrome version",

      "last 1 firefox version",

      "last 1 safari version"

    ]

  }

}

```

  

### ESLint and Prettier Configuration

  

For consistent code style and quality, set up ESLint and Prettier:

  

```bash

npm install --save-dev eslint prettier eslint-config-prettier eslint-plugin-prettier

```

  

#### .eslintrc.js

  

```javascript

module.exports = {

  extends: [

    'react-app',

    'react-app/jest',

    'plugin:prettier/recommended',

  ],

  rules: {

    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',

    'no-debugger': process.env.NODE_ENV === 'production' ? 'warn' : 'off',

    'prettier/prettier': ['error', {}, { usePrettierrc: true }],

  },

};

```

  

#### .prettierrc

  

```json

{

  "semi": true,

  "tabWidth": 2,

  "printWidth": 100,

  "singleQuote": true,

  "trailingComma": "es5",

  "jsxBracketSameLine": false

}

```

  

### Git Configuration

  

Create a `.gitignore` file to exclude unnecessary files from version control:

  

```

# dependencies

/node_modules

/.pnp

.pnp.js

  

# testing

/coverage

  

# production

/build

  

# misc

.DS_Store

.env.local

.env.development.local

.env.test.local

.env.production.local

  

npm-debug.log*

yarn-debug.log*

yarn-error.log*

  

# IDE

.idea

.vscode

*.swp

*.swo

```

  

## 4. Component Hierarchy

  

Designing a well-structured component hierarchy is crucial for maintainable React applications. Based on the backend API structure, we'll design a component hierarchy that reflects the data models and user flows.

  

### Component Hierarchy Overview

  

```

App

├── Layout

│   ├── Header

│   │   ├── Logo

│   │   ├── Navigation

│   │   ├── SearchBar

│   │   ├── CartIcon

│   │   └── UserMenu

│   ├── Main

│   └── Footer

├── Pages

│   ├── Home

│   │   ├── HeroBanner

│   │   ├── FeaturedProducts

│   │   ├── CategoryShowcase

│   │   └── PromotionBanner

│   ├── Products

│   │   ├── ProductList

│   │   │   ├── ProductCard

│   │   │   ├── ProductFilters

│   │   │   └── Pagination

│   │   └── ProductDetail

│   │       ├── ProductGallery

│   │       ├── ProductInfo

│   │       ├── AddToCartForm

│   │       └── ProductReviews

│   ├── Cart

│   │   ├── CartItemList

│   │   │   └── CartItem

│   │   ├── CartSummary

│   │   └── CheckoutButton

│   ├── Checkout

│   │   ├── CheckoutForm

│   │   │   ├── ShippingAddressForm

│   │   │   ├── BillingAddressForm

│   │   │   └── PaymentMethodForm

│   │   └── OrderSummary

│   ├── Orders

│   │   ├── OrderList

│   │   │   └── OrderCard

│   │   └── OrderDetail

│   │       ├── OrderInfo

│   │       ├── OrderItems

│   │       └── OrderStatus

│   ├── Auth

│   │   ├── Login

│   │   └── Register

│   ├── Profile

│   │   ├── UserInfo

│   │   ├── AddressBook

│   │   └── OrderHistory

│   └── Admin

│       ├── Dashboard

│       │   ├── SalesChart

│       │   ├── RecentOrders

│       │   └── InventoryStatus

│       ├── ProductManagement

│       │   ├── ProductTable

│       │   └── ProductForm

│       ├── OrderManagement

│       │   ├── OrderTable

│       │   └── OrderDetailView

│       └── UserManagement

│           ├── UserTable

│           └── UserForm

└── Common Components

    ├── Button

    ├── Input

    ├── Select

    ├── Modal

    ├── Alert

    ├── Loader

    ├── ErrorBoundary

    └── NotFound

```

  

### Component Types

  

1. **Layout Components**: Components that define the overall structure of the application

2. **Page Components**: Top-level components that correspond to different routes

3. **Feature Components**: Components specific to a particular feature or domain

4. **Common Components**: Reusable UI components used across the application

  

### Component Mapping to Backend Entities

  

Based on the backend API structure, here's how the components map to the backend entities:

  

#### User Entity

- UserMenu (Header)

- Login/Register (Auth)

- UserInfo (Profile)

- AddressBook (Profile)

- UserTable (Admin)

- UserForm (Admin)

  

#### Product Entity

- FeaturedProducts (Home)

- ProductCard (ProductList)

- ProductList (Products)

- ProductDetail (Products)

- ProductTable (Admin)

- ProductForm (Admin)

  

#### Category Entity

- CategoryShowcase (Home)

- ProductFilters (ProductList)

  

#### Order Entity

- OrderCard (OrderList)

- OrderDetail (Orders)

- OrderTable (Admin)

- OrderDetailView (Admin)

  

#### ShoppingCart Entity

- CartIcon (Header)

- CartItemList (Cart)

- CartSummary (Cart)

  

#### OrderItem Entity

- OrderItems (OrderDetail)

  

#### CartItem Entity

- CartItem (CartItemList)

  

#### ProductReview Entity

- ProductReviews (ProductDetail)

  

### Detailed Component Specifications

  

#### Layout Components

  

##### Header Component

```jsx

// src/components/layout/Header.jsx

import { AppBar, Toolbar, Typography, Button, IconButton, Badge } from '@mui/material';

import { ShoppingCart, Person } from '@mui/icons-material';

import { Link } from 'react-router-dom';

import { useSelector } from 'react-redux';

import SearchBar from './SearchBar';

import Navigation from './Navigation';

import UserMenu from './UserMenu';

  

const Header = () => {

  const { isAuthenticated, user } = useSelector((state) => state.auth);

  const { items } = useSelector((state) => state.cart);

  return (

    <AppBar position="static">

      <Toolbar>

        <Typography variant="h6" component={Link} to="/" sx={{ flexGrow: 1, textDecoration: 'none', color: 'white' }}>

          Online Store

        </Typography>

        <Navigation />

        <SearchBar />

        <IconButton component={Link} to="/cart" color="inherit">

          <Badge badgeContent={items.length} color="secondary">

            <ShoppingCart />

          </Badge>

        </IconButton>

        {isAuthenticated ? (

          <UserMenu user={user} />

        ) : (

          <Button color="inherit" component={Link} to="/login">

            Login

          </Button>

        )}

      </Toolbar>

    </AppBar>

  );

};

  

export default Header;

```

  

##### Footer Component

```jsx

// src/components/layout/Footer.jsx

import { Box, Container, Grid, Typography, Link } from '@mui/material';

  

const Footer = () => {

  return (

    <Box component="footer" sx={{ bgcolor: 'background.paper', py: 6 }}>

      <Container maxWidth="lg">

        <Grid container spacing={4} justifyContent="space-evenly">

          <Grid item xs={12} sm={4}>

            <Typography variant="h6" color="text.primary" gutterBottom>

              About Us

            </Typography>

            <Typography variant="body2" color="text.secondary">

              We are an online store dedicated to providing quality products at affordable prices.

            </Typography>

          </Grid>

          <Grid item xs={12} sm={4}>

            <Typography variant="h6" color="text.primary" gutterBottom>

              Contact Us

            </Typography>

            <Typography variant="body2" color="text.secondary">

              123 Main Street, Anytown, USA

            </Typography>

            <Typography variant="body2" color="text.secondary">

              Email: info@example.com

            </Typography>

            <Typography variant="body2" color="text.secondary">

              Phone: +1 234 567 8901

            </Typography>

          </Grid>

          <Grid item xs={12} sm={4}>

            <Typography variant="h6" color="text.primary" gutterBottom>

              Follow Us

            </Typography>

            <Link href="https://facebook.com/" color="inherit">

              Facebook

            </Link>

            <br />

            <Link href="https://twitter.com/" color="inherit">

              Twitter

            </Link>

            <br />

            <Link href="https://instagram.com/" color="inherit">

              Instagram

            </Link>

          </Grid>

        </Grid>

        <Box mt={5}>

          <Typography variant="body2" color="text.secondary" align="center">

            {'© '}

            <Link color="inherit" href="/">

              Online Store

            </Link>{' '}

            {new Date().getFullYear()}

            {'.'}

          </Typography>

        </Box>

      </Container>

    </Box>

  );

};

  

export default Footer;

```

  

#### Page Components

  

##### Home Page

```jsx

// src/pages/Home/Home.jsx

import { useEffect } from 'react';

import { Container } from '@mui/material';

import { useDispatch, useSelector } from 'react-redux';

import HeroBanner from './HeroBanner';

import FeaturedProducts from './FeaturedProducts';

import CategoryShowcase from './CategoryShowcase';

import PromotionBanner from './PromotionBanner';

import { fetchFeaturedProducts } from '../../store/slices/productSlice';

import { fetchCategories } from '../../store/slices/categorySlice';

  

const Home = () => {

  const dispatch = useDispatch();

  const { featuredProducts, loading: productsLoading } = useSelector((state) => state.products);

  const { categories, loading: categoriesLoading } = useSelector((state) => state.categories);

  useEffect(() => {

    dispatch(fetchFeaturedProducts());

    dispatch(fetchCategories());

  }, [dispatch]);

  return (

    <Container maxWidth="lg">

      <HeroBanner />

      <FeaturedProducts products={featuredProducts} loading={productsLoading} />

      <CategoryShowcase categories={categories} loading={categoriesLoading} />

      <PromotionBanner />

    </Container>

  );

};

  

export default Home;

```

  

##### Product List Page

```jsx

// src/pages/Products/ProductList.jsx

import { useEffect, useState } from 'react';

import { useDispatch, useSelector } from 'react-redux';

import { useLocation } from 'react-router-dom';

import { Container, Grid, Typography, Box, CircularProgress } from '@mui/material';

import ProductCard from './ProductCard';

import ProductFilters from './ProductFilters';

import Pagination from '../../components/common/Pagination';

import { fetchProducts, fetchCategories } from '../../store/slices/productSlice';

  

const ProductList = () => {

  const dispatch = useDispatch();

  const location = useLocation();

  const queryParams = new URLSearchParams(location.search);

  const categoryId = queryParams.get('category');

  const searchTerm = queryParams.get('search');

  const page = parseInt(queryParams.get('page') || '1', 10);

  const { products, totalCount, pageSize, loading } = useSelector((state) => state.products);

  const { categories } = useSelector((state) => state.categories);

  const [filters, setFilters] = useState({

    category: categoryId || '',

    search: searchTerm || '',

    page: page,

  });

  useEffect(() => {

    dispatch(fetchCategories());

  }, [dispatch]);

  useEffect(() => {

    dispatch(fetchProducts({

      categoryId: filters.category,

      searchTerm: filters.search,

      page: filters.page,

      pageSize: 12,

    }));

  }, [dispatch, filters]);

  const handleFilterChange = (newFilters) => {

    setFilters({ ...filters, ...newFilters, page: 1 });

  };

  const handlePageChange = (newPage) => {

    setFilters({ ...filters, page: newPage });

  };

  return (

    <Container maxWidth="lg">

      <Typography variant="h4" component="h1" gutterBottom>

        Products

      </Typography>

      <Grid container spacing={3}>

        <Grid item xs={12} md={3}>

          <ProductFilters

            categories={categories}

            filters={filters}

            onFilterChange={handleFilterChange}

          />

        </Grid>

        <Grid item xs={12} md={9}>

          {loading ? (

            <Box display="flex" justifyContent="center" my={4}>

              <CircularProgress />

            </Box>

          ) : products.length === 0 ? (

            <Typography variant="h6">No products found.</Typography>

          ) : (

            <>

              <Grid container spacing={2}>

                {products.map((product) => (

                  <Grid item key={product.id} xs={12} sm={6} md={4}>

                    <ProductCard product={product} />

                  </Grid>

                ))}

              </Grid>

              <Box mt={4} display="flex" justifyContent="center">

                <Pagination

                  count={Math.ceil(totalCount / pageSize)}

                  page={filters.page}

                  onChange={handlePageChange}

                />

              </Box>

            </>

          )}

        </Grid>

      </Grid>

    </Container>

  );

};

  

export default ProductList;

```

  

### Data Flow Diagram

  

To better understand how data flows through the application, here's a simplified data flow diagram:

  

```

┌─────────────┐         ┌─────────────┐         ┌─────────────┐

│             │         │             │         │             │

│    Redux    │◄────────│   React     │◄────────│    API      │

│    Store    │         │ Components  │         │  Services   │

│             │─────────►             │─────────►             │

└─────────────┘         └─────────────┘         └─────────────┘

       ▲                                               │

       │                                               │

       │                                               │

       │                                               ▼

       │                                        ┌─────────────┐

       │                                        │             │

       └────────────────────────────────────────│  Backend    │

                                                │    API      │

                                                │             │

                                                └─────────────┘

```

  

1. **API Services**: Make HTTP requests to the backend API

2. **Redux Store**: Manages application state

3. **React Components**: Render UI based on state and dispatch actions

4. **Backend API**: Provides data and business logic

  

### Component Communication Patterns

  

1. **Parent-Child Communication**: Props passing down, callbacks passing up

2. **Component-Store Communication**: Components dispatch actions and select state from the store

3. **Cross-Component Communication**: Through the Redux store or context API

4. **API Communication**: Through service functions that make HTTP requests

  

This component hierarchy provides a solid foundation for building the React frontend that integrates with the .NET Web API backend. The structure is modular, maintainable, and follows best practices for React application development.

  

## 5. Core Components Implementation

  

Now let's implement the core components needed for the application to function properly. We'll focus on the most essential components first.

  

### Common Components

  

#### Button Component

  

```jsx

// src/components/common/Button.jsx

import { Button as MuiButton } from '@mui/material';

import PropTypes from 'prop-types';

  

const Button = ({ children, variant, color, size, fullWidth, disabled, onClick, type, startIcon, endIcon, ...props }) => {

  return (

    <MuiButton

      variant={variant}

      color={color}

      size={size}

      fullWidth={fullWidth}

      disabled={disabled}

      onClick={onClick}

      type={type}

      startIcon={startIcon}

      endIcon={endIcon}

      {...props}

    >

      {children}

    </MuiButton>

  );

};

  

Button.propTypes = {

  children: PropTypes.node.isRequired,

  variant: PropTypes.oneOf(['contained', 'outlined', 'text']),

  color: PropTypes.oneOf(['primary', 'secondary', 'success', 'error', 'info', 'warning']),

  size: PropTypes.oneOf(['small', 'medium', 'large']),

  fullWidth: PropTypes.bool,

  disabled: PropTypes.bool,

  onClick: PropTypes.func,

  type: PropTypes.oneOf(['button', 'submit', 'reset']),

  startIcon: PropTypes.node,

  endIcon: PropTypes.node,

};

  

Button.defaultProps = {

  variant: 'contained',

  color: 'primary',

  size: 'medium',

  fullWidth: false,

  disabled: false,

  type: 'button',

};

  

export default Button;

```

  

#### Input Component

  

```jsx

// src/components/common/Input.jsx

import { TextField } from '@mui/material';

import PropTypes from 'prop-types';

  

const Input = ({

  id,

  name,

  label,

  value,

  onChange,

  type,

  error,

  helperText,

  fullWidth,

  required,

  disabled,

  placeholder,

  variant,

  size,

  ...props

}) => {

  return (

    <TextField

      id={id}

      name={name}

      label={label}

      value={value}

      onChange={onChange}

      type={type}

      error={!!error}

      helperText={error ? error : helperText}

      fullWidth={fullWidth}

      required={required}

      disabled={disabled}

      placeholder={placeholder}

      variant={variant}

      size={size}

      {...props}

    />

  );

};

  

Input.propTypes = {

  id: PropTypes.string,

  name: PropTypes.string.isRequired,

  label: PropTypes.string,

  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),

  onChange: PropTypes.func.isRequired,

  type: PropTypes.string,

  error: PropTypes.string,

  helperText: PropTypes.string,

  fullWidth: PropTypes.bool,

  required: PropTypes.bool,

  disabled: PropTypes.bool,

  placeholder: PropTypes.string,

  variant: PropTypes.oneOf(['outlined', 'filled', 'standard']),

  size: PropTypes.oneOf(['small', 'medium']),

};

  

Input.defaultProps = {

  type: 'text',

  fullWidth: true,

  required: false,

  disabled: false,

  variant: 'outlined',

  size: 'medium',

};

  

export default Input;

```

  

#### Select Component

  

```jsx

// src/components/common/Select.jsx

import { FormControl, InputLabel, Select as MuiSelect, MenuItem, FormHelperText } from '@mui/material';

import PropTypes from 'prop-types';

  

const Select = ({

  id,

  name,

  label,

  value,

  onChange,

  options,

  error,

  helperText,

  fullWidth,

  required,

  disabled,

  variant,

  size,

  ...props

}) => {

  return (

    <FormControl

      fullWidth={fullWidth}

      error={!!error}

      required={required}

      disabled={disabled}

      variant={variant}

      size={size}

    >

      <InputLabel id={`${id}-label`}>{label}</InputLabel>

      <MuiSelect

        labelId={`${id}-label`}

        id={id}

        name={name}

        value={value}

        onChange={onChange}

        label={label}

        {...props}

      >

        {options.map((option) => (

          <MenuItem key={option.value} value={option.value}>

            {option.label}

          </MenuItem>

        ))}

      </MuiSelect>

      {(error || helperText) && <FormHelperText>{error || helperText}</FormHelperText>}

    </FormControl>

  );

};

  

Select.propTypes = {

  id: PropTypes.string,

  name: PropTypes.string.isRequired,

  label: PropTypes.string.isRequired,

  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),

  onChange: PropTypes.func.isRequired,

  options: PropTypes.arrayOf(

    PropTypes.shape({

      value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,

      label: PropTypes.string.isRequired,

    })

  ).isRequired,

  error: PropTypes.string,

  helperText: PropTypes.string,

  fullWidth: PropTypes.bool,

  required: PropTypes.bool,

  disabled: PropTypes.bool,

  variant: PropTypes.oneOf(['outlined', 'filled', 'standard']),

  size: PropTypes.oneOf(['small', 'medium']),

};

  

Select.defaultProps = {

  fullWidth: true,

  required: false,

  disabled: false,

  variant: 'outlined',

  size: 'medium',

  options: [],

};

  

export default Select;

```

  

#### Alert Component

  

```jsx

// src/components/common/Alert.jsx

import { Alert as MuiAlert, Snackbar } from '@mui/material';

import PropTypes from 'prop-types';

  

const Alert = ({ open, message, severity, onClose, autoHideDuration }) => {

  return (

    <Snackbar open={open} autoHideDuration={autoHideDuration} onClose={onClose}>

      <MuiAlert elevation={6} variant="filled" onClose={onClose} severity={severity}>

        {message}

      </MuiAlert>

    </Snackbar>

  );

};

  

Alert.propTypes = {

  open: PropTypes.bool.isRequired,

  message: PropTypes.string.isRequired,

  severity: PropTypes.oneOf(['success', 'info', 'warning', 'error']),

  onClose: PropTypes.func.isRequired,

  autoHideDuration: PropTypes.number,

};

  

Alert.defaultProps = {

  severity: 'info',

  autoHideDuration: 6000,

};

  

export default Alert;

```

  

#### Loader Component

  

```jsx

// src/components/common/Loader.jsx

import { CircularProgress, Box, Typography } from '@mui/material';

import PropTypes from 'prop-types';

  

const Loader = ({ size, color, text }) => {

  return (

    <Box display="flex" flexDirection="column" alignItems="center" justifyContent="center" p={3}>

      <CircularProgress size={size} color={color} />

      {text && (

        <Typography variant="body2" color="textSecondary" mt={2}>

          {text}

        </Typography>

      )}

    </Box>

  );

};

  

Loader.propTypes = {

  size: PropTypes.number,

  color: PropTypes.oneOf(['primary', 'secondary', 'success', 'error', 'info', 'warning', 'inherit']),

  text: PropTypes.string,

};

  

Loader.defaultProps = {

  size: 40,

  color: 'primary',

};

  

export default Loader;

```

  

#### Pagination Component

  

```jsx

// src/components/common/Pagination.jsx

import { Pagination as MuiPagination } from '@mui/material';

import PropTypes from 'prop-types';

  

const Pagination = ({ count, page, onChange, color, size, showFirstButton, showLastButton }) => {

  const handleChange = (event, value) => {

    onChange(value);

  };

  

  return (

    <MuiPagination

      count={count}

      page={page}

      onChange={handleChange}

      color={color}

      size={size}

      showFirstButton={showFirstButton}

      showLastButton={showLastButton}

    />

  );

};

  

Pagination.propTypes = {

  count: PropTypes.number.isRequired,

  page: PropTypes.number.isRequired,

  onChange: PropTypes.func.isRequired,

  color: PropTypes.oneOf(['primary', 'secondary', 'standard']),

  size: PropTypes.oneOf(['small', 'medium', 'large']),

  showFirstButton: PropTypes.bool,

  showLastButton: PropTypes.bool,

};

  

Pagination.defaultProps = {

  color: 'primary',

  size: 'medium',

  showFirstButton: true,

  showLastButton: true,

};

  

export default Pagination;

```

  

### Feature Components

  

#### ProductCard Component

  

```jsx

// src/components/features/ProductCard.jsx

import { Card, CardMedia, CardContent, CardActions, Typography, Button, Rating, Box } from '@mui/material';

import { Link } from 'react-router-dom';

import { useDispatch } from 'react-redux';

import { addToCart } from '../../store/slices/cartSlice';

import PropTypes from 'prop-types';

  

const ProductCard = ({ product }) => {

  const dispatch = useDispatch();

  const handleAddToCart = () => {

    dispatch(addToCart({

      productId: product.id,

      quantity: 1

    }));

  };

  return (

    <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>

      <CardMedia

        component="img"

        height="200"

        image={product.imageUrl || 'https://via.placeholder.com/200'}

        alt={product.name}

      />

      <CardContent sx={{ flexGrow: 1 }}>

        <Typography gutterBottom variant="h6" component="div" noWrap>

          {product.name}

        </Typography>

        <Typography variant="body2" color="text.secondary" sx={{ mb: 1, height: '40px', overflow: 'hidden' }}>

          {product.description}

        </Typography>

        <Box display="flex" alignItems="center" mb={1}>

          <Rating value={product.rating || 0} precision={0.5} readOnly size="small" />

          <Typography variant="body2" color="text.secondary" ml={1}>

            ({product.reviewCount || 0})

          </Typography>

        </Box>

        <Typography variant="h6" color="primary">

          ${product.price.toFixed(2)}

        </Typography>

      </CardContent>

      <CardActions>

        <Button size="small" component={Link} to={`/products/${product.id}`}>

          View Details

        </Button>

        <Button

          size="small"

          color="primary"

          onClick={handleAddToCart}

          disabled={product.stockQuantity <= 0}

        >

          {product.stockQuantity > 0 ? 'Add to Cart' : 'Out of Stock'}

        </Button>

      </CardActions>

    </Card>

  );

};

  

ProductCard.propTypes = {

  product: PropTypes.shape({

    id: PropTypes.string.isRequired,

    name: PropTypes.string.isRequired,

    description: PropTypes.string,

    price: PropTypes.number.isRequired,

    imageUrl: PropTypes.string,

    stockQuantity: PropTypes.number.isRequired,

    rating: PropTypes.number,

    reviewCount: PropTypes.number,

  }).isRequired,

};

  

export default ProductCard;

```

  

#### CartItem Component

  

```jsx

// src/components/features/CartItem.jsx

import { useState } from 'react';

import { Box, Card, CardMedia, Typography, IconButton, TextField, Grid } from '@mui/material';

import { Delete, Add, Remove } from '@mui/icons-material';

import { useDispatch } from 'react-redux';

import { updateCartItem, removeFromCart } from '../../store/slices/cartSlice';

import PropTypes from 'prop-types';

  

const CartItem = ({ item }) => {

  const dispatch = useDispatch();

  const [quantity, setQuantity] = useState(item.quantity);

  const handleQuantityChange = (event) => {

    const newQuantity = parseInt(event.target.value, 10);

    if (newQuantity > 0 && newQuantity <= item.product.stockQuantity) {

      setQuantity(newQuantity);

      dispatch(updateCartItem({

        productId: item.product.id,

        quantity: newQuantity

      }));

    }

  };

  const handleIncrement = () => {

    if (quantity < item.product.stockQuantity) {

      const newQuantity = quantity + 1;

      setQuantity(newQuantity);

      dispatch(updateCartItem({

        productId: item.product.id,

        quantity: newQuantity

      }));

    }

  };

  const handleDecrement = () => {

    if (quantity > 1) {

      const newQuantity = quantity - 1;

      setQuantity(newQuantity);

      dispatch(updateCartItem({

        productId: item.product.id,

        quantity: newQuantity

      }));

    }

  };

  const handleRemove = () => {

    dispatch(removeFromCart(item.product.id));

  };

  return (

    <Card sx={{ mb: 2, p: 2 }}>

      <Grid container spacing={2} alignItems="center">

        <Grid item xs={12} sm={3}>

          <CardMedia

            component="img"

            height="100"

            image={item.product.imageUrl || 'https://via.placeholder.com/100'}

            alt={item.product.name}

            sx={{ objectFit: 'contain' }}

          />

        </Grid>

        <Grid item xs={12} sm={4}>

          <Typography variant="subtitle1" component="div">

            {item.product.name}

          </Typography>

          <Typography variant="body2" color="text.secondary">

            ${item.product.price.toFixed(2)} each

          </Typography>

        </Grid>

        <Grid item xs={12} sm={3}>

          <Box display="flex" alignItems="center">

            <IconButton size="small" onClick={handleDecrement} disabled={quantity <= 1}>

              <Remove fontSize="small" />

            </IconButton>

            <TextField

              size="small"

              value={quantity}

              onChange={handleQuantityChange}

              inputProps={{

                min: 1,

                max: item.product.stockQuantity,

                style: { textAlign: 'center' }

              }}

              sx={{ width: '60px', mx: 1 }}

            />

            <IconButton size="small" onClick={handleIncrement} disabled={quantity >= item.product.stockQuantity}>

              <Add fontSize="small" />

            </IconButton>

          </Box>

        </Grid>

        <Grid item xs={12} sm={1}>

          <Typography variant="subtitle1" align="right">

            ${(item.product.price * quantity).toFixed(2)}

          </Typography>

        </Grid>

        <Grid item xs={12} sm={1}>

          <IconButton color="error" onClick={handleRemove}>

            <Delete />

          </IconButton>

        </Grid>

      </Grid>

    </Card>

  );

};

  

CartItem.propTypes = {

  item: PropTypes.shape({

    product: PropTypes.shape({

      id: PropTypes.string.isRequired,

      name: PropTypes.string.isRequired,

      price: PropTypes.number.isRequired,

      imageUrl: PropTypes.string,

      stockQuantity: PropTypes.number.isRequired,

    }).isRequired,

    quantity: PropTypes.number.isRequired,

  }).isRequired,

};

  

export default CartItem;

```

  

#### ProductFilters Component

  

```jsx

// src/components/features/ProductFilters.jsx

import { useState } from 'react';

import {

  Paper,

  Typography,

  Divider,

  FormControl,

  FormGroup,

  FormControlLabel,

  Checkbox,

  Slider,

  TextField,

  Button,

  Box

} from '@mui/material';

import PropTypes from 'prop-types';

  

const ProductFilters = ({ categories, filters, onFilterChange }) => {

  const [priceRange, setPriceRange] = useState([0, 1000]);

  const [localFilters, setLocalFilters] = useState({

    category: filters.category || '',

    minPrice: filters.minPrice || 0,

    maxPrice: filters.maxPrice || 1000,

    search: filters.search || '',

  });

  const handleCategoryChange = (event) => {

    const { value, checked } = event.target;

    setLocalFilters({

      ...localFilters,

      category: checked ? value : '',

    });

  };

  const handlePriceChange = (event, newValue) => {

    setPriceRange(newValue);

    setLocalFilters({

      ...localFilters,

      minPrice: newValue[0],

      maxPrice: newValue[1],

    });

  };

  const handleSearchChange = (event) => {

    setLocalFilters({

      ...localFilters,

      search: event.target.value,

    });

  };

  const handleApplyFilters = () => {

    onFilterChange(localFilters);

  };

  const handleResetFilters = () => {

    setPriceRange([0, 1000]);

    setLocalFilters({

      category: '',

      minPrice: 0,

      maxPrice: 1000,

      search: '',

    });

    onFilterChange({

      category: '',

      minPrice: 0,

      maxPrice: 1000,

      search: '',

    });

  };

  return (

    <Paper elevation={2} sx={{ p: 2 }}>

      <Typography variant="h6" gutterBottom>

        Filters

      </Typography>

      <Divider sx={{ my: 2 }} />

      <Typography variant="subtitle1" gutterBottom>

        Search

      </Typography>

      <TextField

        fullWidth

        size="small"

        placeholder="Search products..."

        value={localFilters.search}

        onChange={handleSearchChange}

        sx={{ mb: 2 }}

      />

      <Divider sx={{ my: 2 }} />

      <Typography variant="subtitle1" gutterBottom>

        Categories

      </Typography>

      <FormControl component="fieldset">

        <FormGroup>

          {categories.map((category) => (

            <FormControlLabel

              key={category.id}

              control={

                <Checkbox

                  checked={localFilters.category === category.id}

                  onChange={handleCategoryChange}

                  value={category.id}

                />

              }

              label={category.name}

            />

          ))}

        </FormGroup>

      </FormControl>

      <Divider sx={{ my: 2 }} />

      <Typography variant="subtitle1" gutterBottom>

        Price Range

      </Typography>

      <Box px={1}>

        <Slider

          value={priceRange}

          onChange={handlePriceChange}

          valueLabelDisplay="auto"

          min={0}

          max={1000}

          step={10}

        />

        <Box display="flex" justifyContent="space-between">

          <Typography variant="body2">${priceRange[0]}</Typography>

          <Typography variant="body2">${priceRange[1]}</Typography>

        </Box>

      </Box>

      <Divider sx={{ my: 2 }} />

      <Box display="flex" justifyContent="space-between">

        <Button variant="outlined" size="small" onClick={handleResetFilters}>

          Reset

        </Button>

        <Button variant="contained" size="small" onClick={handleApplyFilters}>

          Apply Filters

        </Button>

      </Box>

    </Paper>

  );

};

  

ProductFilters.propTypes = {

  categories: PropTypes.arrayOf(

    PropTypes.shape({

      id: PropTypes.string.isRequired,

      name: PropTypes.string.isRequired,

    })

  ).isRequired,

  filters: PropTypes.shape({

    category: PropTypes.string,

    minPrice: PropTypes.number,

    maxPrice: PropTypes.number,

    search: PropTypes.string,

  }).isRequired,

  onFilterChange: PropTypes.func.isRequired,

};

  

ProductFilters.defaultProps = {

  categories: [],

  filters: {

    category: '',

    minPrice: 0,

    maxPrice: 1000,

    search: '',

  },

};

  

export default ProductFilters;

```

  

### Authentication Components

  

#### Login Component

  

```jsx

// src/pages/Auth/Login.jsx

import { useState } from 'react';

import { useDispatch, useSelector } from 'react-redux';

import { Link as RouterLink, useNavigate } from 'react-router-dom';

import {

  Container,

  Paper,

  Typography,

  TextField,

  Button,

  Link,

  Box,

  CircularProgress,

  Alert

} from '@mui/material';

import { login } from '../../store/slices/authSlice';

  

const Login = () => {

  const dispatch = useDispatch();

  const navigate = useNavigate();

  const { loading, error } = useSelector((state) => state.auth);

  const [formData, setFormData] = useState({

    email: '',

    password: '',

  });

  const handleChange = (e) => {

    const { name, value } = e.target;

    setFormData({

      ...formData,

      [name]: value,

    });

  };

  const handleSubmit = async (e) => {

    e.preventDefault();

    const resultAction = await dispatch(login(formData));

    if (login.fulfilled.match(resultAction)) {

      navigate('/');

    }

  };

  return (

    <Container maxWidth="sm">

      <Paper elevation={3} sx={{ p: 4, mt: 8 }}>

        <Typography variant="h4" component="h1" align="center" gutterBottom>

          Login

        </Typography>

        {error && (

          <Alert severity="error" sx={{ mb: 2 }}>

            {error}

          </Alert>

        )}

        <Box component="form" onSubmit={handleSubmit} noValidate>

          <TextField

            margin="normal"

            required

            fullWidth

            id="email"

            label="Email Address"

            name="email"

            autoComplete="email"

            autoFocus

            value={formData.email}

            onChange={handleChange}

          />

          <TextField

            margin="normal"

            required

            fullWidth

            name="password"

            label="Password"

            type="password"

            id="password"

            autoComplete="current-password"

            value={formData.password}

            onChange={handleChange}

          />

          <Button

            type="submit"

            fullWidth

            variant="contained"

            sx={{ mt: 3, mb: 2 }}

            disabled={loading}

          >

            {loading ? <CircularProgress size={24} /> : 'Sign In'}

          </Button>

          <Box display="flex" justifyContent="space-between">

            <Link component={RouterLink} to="/forgot-password" variant="body2">

              Forgot password?

            </Link>

            <Link component={RouterLink} to="/register" variant="body2">

              {"Don't have an account? Sign Up"}

            </Link>

          </Box>

        </Box>

      </Paper>

    </Container>

  );

};

  

export default Login;

```

  

#### Register Component

  

```jsx

// src/pages/Auth/Register.jsx

import { useState } from 'react';

import { useDispatch, useSelector } from 'react-redux';

import { Link as RouterLink, useNavigate } from 'react-router-dom';

import {

  Container,

  Paper,

  Typography,

  TextField,

  Button,

  Link,

  Box,

  CircularProgress,

  Alert,

  Grid

} from '@mui/material';

import { register } from '../../store/slices/authSlice';

  

const Register = () => {

  const dispatch = useDispatch();

  const navigate = useNavigate();

  const { loading, error } = useSelector((state) => state.auth);

  const [formData, setFormData] = useState({

    firstName: '',

    lastName: '',

    email: '',

    username: '',

    password: '',

    confirmPassword: '',

  });

  const [formErrors, setFormErrors] = useState({});

  const handleChange = (e) => {

    const { name, value } = e.target;

    setFormData({

      ...formData,

      [name]: value,

    });

    // Clear error when field is edited

    if (formErrors[name]) {

      setFormErrors({

        ...formErrors,

        [name]: '',

      });

    }

  };

  const validateForm = () => {

    const errors = {};

    if (!formData.firstName.trim()) {

      errors.firstName = 'First name is required';

    }

    if (!formData.lastName.trim()) {

      errors.lastName = 'Last name is required';

    }

    if (!formData.email.trim()) {

      errors.email = 'Email is required';

    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {

      errors.email = 'Email is invalid';

    }

    if (!formData.username.trim()) {

      errors.username = 'Username is required';

    } else if (formData.username.length < 4) {

      errors.username = 'Username must be at least 4 characters';

    }

    if (!formData.password) {

      errors.password = 'Password is required';

    } else if (formData.password.length < 6) {

      errors.password = 'Password must be at least 6 characters';

    }

    if (formData.password !== formData.confirmPassword) {

      errors.confirmPassword = 'Passwords do not match';

    }

    setFormErrors(errors);

    return Object.keys(errors).length === 0;

  };

  const handleSubmit = async (e) => {

    e.preventDefault();

    if (validateForm()) {

      const resultAction = await dispatch(register(formData));

      if (register.fulfilled.match(resultAction)) {

        navigate('/login');

      }

    }

  };

  return (

    <Container maxWidth="md">

      <Paper elevation={3} sx={{ p: 4, mt: 8 }}>

        <Typography variant="h4" component="h1" align="center" gutterBottom>

          Create an Account

        </Typography>

        {error && (

          <Alert severity="error" sx={{ mb: 2 }}>

            {error}

          </Alert>

        )}

        <Box component="form" onSubmit={handleSubmit} noValidate>

          <Grid container spacing={2}>

            <Grid item xs={12} sm={6}>

              <TextField

                margin="normal"

                required

                fullWidth

                id="firstName"

                label="First Name"

                name="firstName"

                autoComplete="given-name"

                autoFocus

                value={formData.firstName}

                onChange={handleChange}

                error={!!formErrors.firstName}

                helperText={formErrors.firstName}

              />

            </Grid>

            <Grid item xs={12} sm={6}>

              <TextField

                margin="normal"

                required

                fullWidth

                id="lastName"

                label="Last Name"

                name="lastName"

                autoComplete="family-name"

                value={formData.lastName}

                onChange={handleChange}

                error={!!formErrors.lastName}

                helperText={formErrors.lastName}

              />

            </Grid>

            <Grid item xs={12}>

              <TextField

                margin="normal"

                required

                fullWidth

                id="email"

                label="Email Address"

                name="email"

                autoComplete="email"

                value={formData.email}

                onChange={handleChange}

                error={!!formErrors.email}

                helperText={formErrors.email}

              />

            </Grid>

            <Grid item xs={12}>

              <TextField

                margin="normal"

                required

                fullWidth

                id="username"

                label="Username"

                name="username"

                autoComplete="username"

                value={formData.username}

                onChange={handleChange}

                error={!!formErrors.username}

                helperText={formErrors.username}

              />

            </Grid>

            <Grid item xs={12} sm={6}>

              <TextField

                margin="normal"

                required

                fullWidth

                name="password"

                label="Password"

                type="password"

                id="password"

                autoComplete="new-password"

                value={formData.password}

                onChange={handleChange}

                error={!!formErrors.password}

                helperText={formErrors.password}

              />

            </Grid>

            <Grid item xs={12} sm={6}>

              <TextField

                margin="normal"

                required

                fullWidth

                name="confirmPassword"

                label="Confirm Password"

                type="password"

                id="confirmPassword"

                value={formData.confirmPassword}

                onChange={handleChange}

                error={!!formErrors.confirmPassword}

                helperText={formErrors.confirmPassword}

              />

            </Grid>

          </Grid>

          <Button

            type="submit"

            fullWidth

            variant="contained"

            sx={{ mt: 3, mb: 2 }}

            disabled={loading}

          >

            {loading ? <CircularProgress size={24} /> : 'Sign Up'}

          </Button>

          <Box display="flex" justifyContent="center">

            <Link component={RouterLink} to="/login" variant="body2">

              Already have an account? Sign in

            </Link>

          </Box>

        </Box>

      </Paper>

    </Container>

  );

};

  

export default Register;

```

  

These core components provide the foundation for the React frontend application. They include common UI components, feature-specific components, and authentication components that will be used throughout the application. In the next section, we'll implement the API integration to connect these components to the backend.