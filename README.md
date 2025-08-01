ExchangeAppCS
ExchangeAppCS — a user-friendly desktop application for managing the exchange of personal items. Built with C# and WPF using the MVVM pattern, it facilitates easy item listing, browsing, and secure exchange offers between users.

🚀 Features
User Authentication: Register and log in with phone number and password.

Role-Based Access: Separate interfaces for clients and administrators.

Item Management: Add, edit, and delete your items for exchange.

Exchange Offers: Create, view, accept, reject, or delete exchange proposals.

Advanced Filtering: Browse and filter items by categories.

Notifications: Track and manage exchange requests and notifications.

Password Recovery: Reset your password using your registered phone number.

Admin Panel: Manage users and items — view, modify, or delete entries.

Clean UI: Intuitive and modern interface optimized for user experience.

🏗️ Architecture Overview
Client-Server Model: The client app connects to a server hosting a MySQL database.

Database: Stores data in three main tables — users, items, and exchangeoffers.

Data Integrity: Foreign keys link users to items and exchange offers.

Entity Framework: Used for ORM within the MVVM design pattern for clean separation of concerns.

Real-Time Updates: Maintains data consistency and updates during exchanges.

📂 Database Structure
Table Name	Description
users	User details: ID, username, hashed password.
items	Item details: ID, owner ID, name, description, category, image link, exchange category.
exchangeoffers	Exchange requests: offered item ID, requested item ID, sender, receiver, status (new, accepted, rejected).

🎯 How It Works
Sign Up / Login: Create an account or log into an existing one.

Explore Items: Browse items available for exchange or filter by category.

Manage Your Items: Add, edit, or remove your items.

Exchange Process: Create exchange proposals or respond to offers on your items.

Admin Controls: Admin users can oversee all users and items, managing the system’s integrity.

🛠️ Technologies Used
C# with WPF

MVVM Design Pattern

MySQL Database

Entity Framework

.NET Framework / .NET Core

