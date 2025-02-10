# Getting Started

This guide helps you set up and deploy the **Back Blog Post Manager**.

## Prerequisites
- **Docker & Docker Compose**: For containerized deployment.
- **.NET SDK 8.0 or higher**: For local development.

## Setup Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/fz3hra/dotnet_blog_post_manager.git
   cd dotnet_blog_post_manager
   ```

2. **Build Docker Image**
   ```bash
   docker-compose build
   ```

3. **Run the Application**
   ```bash
   docker-compose up -d
   ```

4. **Access the APIs**
   ```text
   User.API: http://localhost:8081/swagger
   Post.API: http://localhost:8080/swagger
   ```
