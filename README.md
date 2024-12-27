# Authentication Service

This repository implements a secure and scalable **Authentication Service** using **ASP.NET Core** and **N-Tier Architecture**. It provides user authentication, signup, password reset, and JWT-based authentication with role-based access control for modern web applications.

---

## N-Tier Architecture

The project is structured into multiple layers to ensure scalability, maintainability, and separation of concerns:

1. **Presentation Layer**: Handles API endpoints and user interaction.
2. **Business Logic Layer**: Contains service classes implementing core application logic.
3. **Data Access Layer**: Manages interactions with the database using Entity Framework Core.
4. **Shared Layer**: Includes DTOs, utilities, and shared configurations.

---

## Features

- **User Authentication**  
  - Login with email, username, or phone number.  
  - Multi-factor validation.  

- **User Registration (Signup)**  
  - Register new users with role assignment.  
  - Prevent duplicate email or phone number entries.  

- **Password Management**  
  - Reset passwords with secure token validation.  
  - Ensure password and confirm password matching.

- **JWT Token Management**  
  - Generate access and refresh tokens with expiration handling.  
  - Include claims for user roles and permissions.

- **First-Time Login Support**  
  - Provide user-friendly responses for first-time logins.  

- **Security Features**  
  - Use SHA256 hashing and encryption.  
  - Implement role-based access control via claims.

---

## Technologies Used

- **ASP.NET Core**  
- **Entity Framework Core**  
- **ASP.NET Core Identity**  
- **AutoMapper**  
- **JWT (JSON Web Tokens)**  
- **N-Tier Architecture**

---
