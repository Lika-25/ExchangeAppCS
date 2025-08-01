# ExchangeAppCS

**ExchangeAppCS** is a user-friendly desktop application for managing the exchange of personal items. Built with C# and WPF using the MVVM pattern, it simplifies listing, browsing, and exchanging items securely between users.

## Features

- User registration and login with phone number and password
- Role-based access: Client and Administrator interfaces
- Add, edit, and delete your own items for exchange
- Create, view, accept, reject, and delete exchange offers
- Filter items by categories for easy browsing
- Notifications for exchange requests and updates
- Password recovery using registered phone number
- Administrator panel for managing users and items
- Intuitive and clean user interface

## Architecture

- Client-server architecture with a MySQL database backend
- Three main database tables: `users`, `items`, `exchangeoffers`
- Entity Framework integration for database operations
- MVVM design pattern to separate UI and business logic
- Data integrity ensured by foreign keys between tables
- Real-time updates of exchange status and offers

## Database Structure

| Table Name       | Description                                                 |
|------------------|-------------------------------------------------------------|
| `users`          | Stores user info: ID, username, hashed password             |
| `items`          | Stores items: ID, owner ID, name, description, category, image URL, exchange category |
| `exchangeoffers` | Stores exchange proposals: offered item ID, requested item ID, sender ID, receiver ID, status (new, accepted, rejected) |

## How to Use

1. Register or login with your phone number and password.
2. Browse available items or filter by category.
3. Add, edit, or remove your own items.
4. Create exchange offers or respond to existing ones.
5. Administrators can manage users and items through the admin panel.

## Technologies

- C# with WPF
- .NET Framework / .NET Core
- MySQL Database
- Entity Framework
- MVVM Design Pattern
