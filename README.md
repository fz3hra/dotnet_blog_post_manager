# Blog Post Manager

The Blog Post Manager is a full-stack application designed to provide comprehensive blog post management capabilities. The system is built using React for the frontend and .NET microservices for the backend, offering a robust and scalable solution for content management.

## Project Overview

The application uses a microservices architecture with separate services handling user authentication and post management. The frontend provides an intuitive interface for content creation and management, while the backend ensures secure data handling and storage.

## Running the Project

### Frontend Setup

The frontend application requires Node.js version 14 or higher. To get started:

Install the dependencies and start the development server:

```bash
npm install
npm run dev
```

The frontend application will be available at http://localhost:5173.

### Backend Setup

The backend services can be run using either Docker or manual setup.

For Docker deployment:

```bash
docker-compose up -d
```

This command starts three containers:

- SQL Server database
- Post management service
- User authentication service

For manual setup, configure the database connection in `appsettings.Development.json` for both API services:

```json
{
  "ConnectionStrings": {
    "UserDbConnectionString": "Server=localhost,1433;Database=BlogDb;TrustServerCertificate=True;Trusted_Connection=False;Encrypt=False;MultipleActiveResultSets=true;Integrated Security=False;User ID=sa;Password=YourPassword"
  }
}
```

Start each service individually:

```bash
cd Post.API
dotnet run

cd ../User.API
dotnet run
```

## Design Choices

Our architectural decisions reflect a focus on maintainability, scalability, and security.

In the frontend, we've implemented a component-based architecture using React with TypeScript. The application uses React Context for state management, providing a balance between complexity and functionality. Authentication state is managed globally, while component-specific state remains local to ensure optimal performance.

The backend implements a microservices architecture with two primary services. The Post.API handles all blog post operations, while the User.API manages authentication and user data. This separation allows for independent scaling and maintenance of each service.

Data persistence is handled through Entity Framework Core with SQL Server, implementing a code-first approach for better version control and maintainability. The system uses JWT tokens for authentication, ensuring secure communication between the frontend and backend services.

## Features

The Blog Post Manager includes several key features designed to provide a comprehensive content management experience.

The authentication system provides secure user management with JWT-based authentication. Users can register accounts, log in securely, and maintain authenticated sessions. The system implements role-based authorization to control access to various features.

The post management system allows users to create, edit, and delete blog posts. Authors can save posts as drafts or publish them immediately. The system supports featured images and tags for better content organization. Each post tracks view counts and maintains metadata about creation and modification times.

The user interface implements responsive design principles, ensuring a consistent experience across different devices. The application provides real-time feedback during content creation and editing, with automatic draft saving to prevent content loss.

## Directory Structure

The project is organized into clearly defined directories:

```
dotnet_blog_post_manager/
├── Post.API/                # Post management service
│   ├── Configurations/      # AutoMapper and other configurations
│   ├── Contracts/          # Interfaces
│   ├── Controllers/        # API endpoints
│   ├── Data/               # Database context and models
│   ├── ModelDtos/          # Data transfer objects
│   └── Repository/         # Data access implementation
├── Post.API.Tests/          # Tests for Post API
├── User.API/                # User authentication service
│   ├── Configurations/      # AutoMapper and other configurations
│   ├── Contracts/          # Interfaces
│   ├── Controllers/        # API endpoints
│   ├── Data/               # Database context and models
│   ├── ModelDtos/          # Data transfer objects
│   └── Repository/         # Data access implementation
├── User.API.Tests/          # Tests for User API
└── docker-compose.yml       # Docker configuration
```

## API Documentation

The backend services provide Swagger documentation accessible at:

- Post.API: http://localhost:8080/swagger
- User.API: http://localhost:8081/swagger

These endpoints provide detailed information about available API routes and request/response formats.

## Configuration

The application supports different environments through configuration files. The backend uses `appsettings.json` and `appsettings.Development.json` for environment-specific settings. The frontend uses environment variables through `.env` files.

For production deployment, ensure all sensitive information is properly secured and environment variables are appropriately set.

## Testing

The application includes test coverage for backend components. Execute backend tests using:

```bash
dotnet test Post.API.Tests
dotnet test User.API.Tests
```

## Support

For technical support or questions about the implementation, please create an issue in the repository with detailed information about your concern. Include relevant logs and steps to reproduce any issues you encounter.
