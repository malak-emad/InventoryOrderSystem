# Inventory & Order Management System

## Overview

This is an ASP.NET Core MVC application for managing products, categories, and customer orders.

The application uses role-based access with two roles:

- Admin
- User

## Technologies

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Bootstrap

## Main Features

### Admin
- Manage products
- Manage categories
- View all orders
- Confirm orders
- Cancel orders

### User
- View active products
- Search products by name
- Filter products by category
- Create orders
- View their own orders and order details

## Authentication and Authorization

The application uses ASP.NET Core Identity for authentication and role-based authorization.

Admins can access the product and category management pages, while users can create orders and view their own orders.

## Database

Entity Framework Core is used to communicate with SQL Server.

The main entities are:

- Product
- Category
- Order
- OrderItem
- ApplicationUser

Database migrations are included in the project.

## How to Run

1. Open the project in Visual Studio or VS Code.
2. Make sure SQL Server is available.
3. Check the connection string in `appsettings.json`.
4. Apply the database migrations.
5. Run the application.

## Test Accounts

### Admin
Email: `admin@test.com`  
Password: `Admin1234`

### User
Email: `user@test.com`  
Password: `User123`

### Second User
Email: `user2@test.com`  
Password: `User123`
