ExchangeAppCS
ExchangeAppCS — это приложение для обмена вещами между пользователями, разработанное на C# с использованием WPF и архитектуры MVVM.

Overview
This application is a platform that allows users to create offers for exchanging personal items. Each user can list items they own and specify which items they are willing to exchange for. If there are exchange offers for an item, the user can view and select from those offers.

The program provides a convenient interface to search, view, and manage items and exchange requests, facilitating efficient interaction between users.

Features
User Registration & Login: Users can register an account using phone number and password, then log in.

User Roles: Separate interfaces and permissions for clients and administrators.

Item Management: Users can add, edit, and delete their own items available for exchange.

Exchange Offers: Users can view exchange offers on their items, create new offers, accept, reject, or delete existing ones.

Item Browsing: Browse available items with filtering by category.

Notifications: View and manage exchange requests and notifications.

Administrator Tools: Admins can manage users and items — view, edit, or delete records.

Password Recovery: Reset password functionality via phone number.

Clean and Intuitive UI: User-friendly navigation and modern interface.

System Architecture
Client-server architecture.

The client app communicates with a server hosting a MySQL database.

Database tables: users, items, exchangeoffers.

Uses Entity Framework for database operations within the MVVM pattern.

Ensures data integrity with foreign key relationships between users, items, and exchange offers.

Database Structure
users: stores user info — id, username, hashed password.

items: stores item info — id, owner user_id, name, description, category, image link, exchange category.

exchangeoffers: stores exchange requests — offered item id, requested item id, sender, receiver, and status (new, accepted, rejected).

How to Use
Register a new account or log in.

Browse items available for exchange or manage your own items.

Create exchange offers or respond to existing ones.

Administrators manage the system users and items via dedicated interfaces.

Technologies
C# with WPF

MVVM design pattern

MySQL database

Entity Framework

.NET Framework / .NET Core (specify your target)
