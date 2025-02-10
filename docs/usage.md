# Auth and Post API Usage Documentation

## Introduction

The **Post API** provides functionalities to manage posts in a database. The repository pattern is used to encapsulate data access logic. This document describes the API endpoints, their expected requests, and responses.

## Dependencies

This API uses the following dependencies:
- `AutoMapper` for object mapping
- `Entity Framework Core` for database operations
- `ILogger` for logging

## Authorization and Headers

All API endpoints require the following headers:

- `Authorization`: `Bearer <token>` - A valid Bearer token for authentication.
- `Content-Type`: `application/json` - Indicates that the request body is in JSON format.

---

## Authentication API

### 1. Register a User

**Endpoint:**
```http
POST /api/Auth/register
```

**Headers:**
```json
{
  "Content-Type": "application/json"
}
```

**Request Body:**
```json
{
  "email": "jdoe@gmail.com",
  "password": "Admin123!",
  "firstName": "John",
  "lastName": "Doe",
  "userName": "jdoe"
}
```

**Response:**
```json
{
  "success": true,
  "message": "User registered successfully"
}
```

**Error Responses:**
- `400 Bad Request`: Missing or invalid fields.
- `409 Conflict`: Email already exists.

### 2. Login a User

**Endpoint:**
```http
POST /api/Auth/login
```

**Headers:**
```json
{
  "Content-Type": "application/json"
}
```

**Request Body:**
```json
{
  "email": "jdoe@gmail.com",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "userId": "e3a9df31-90ce-4a2f-93a7-ceafc7401131",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Responses:**
- `401 Unauthorized`: Incorrect credentials.
- `500 Internal Server Error`: Login service unavailable.

---

## Post API

### 1. Create a Post

**Endpoint:**
```http
POST /api/posts
```

**Headers:**
```json
{
  "Authorization": "Bearer <token>",
  "Content-Type": "application/json"
}
```

**Request Body:**
```json
{
  "title": "Sample Post",
  "description": "This is a test post.",
  "tags": ["tech", "blog"],
  "featuredImageUrl": "https://example.com/image.jpg",
  "excerpt": "Short description of the post.",
  "isPublished": true
}
```

**Response:**
```json
{
  "success": true,
  "message": "Post created successfully",
  "post": {
    "title": "Sample Post",
    "description": "This is a test post.",
    "tags": ["tech", "blog"],
    "featuredImageUrl": "https://example.com/image.jpg",
    "excerpt": "Short description of the post.",
    "id": 1,
    "createdAt": "2025-02-10T08:03:19.883Z",
    "createdBy": "jdoe",
    "status": 0,
    "viewCount": 0,
    "isPublished": true,
    "lastModifiedAt": "2025-02-10T08:03:19.883Z",
    "lastModifiedBy": "jdoe"
  }
}
```

**Error Responses:**
- `400 Bad Request`: Missing required fields.
- `401 Unauthorized`: Invalid or missing Bearer token.
- `500 Internal Server Error`: Database issue.

### 2. Get All Posts

**Endpoint:**
```http
GET /api/posts
```

**Headers:**
```json
{
  "Authorization": "Bearer <token>",
  "Content-Type": "application/json"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Posts retrieved successfully",
  "posts": [
    {
      "id": 1,
      "title": "Sample Post",
      "description": "This is a test post.",
      "tags": ["tech", "blog"],
      "createdAt": "2025-02-10T08:03:19.883Z",
      "createdBy": "jdoe",
      "status": 0,
      "viewCount": 10,
      "isPublished": true
    }
  ]
}
```

### 3. Get Post by ID

**Endpoint:**
```http
GET /api/posts/{id}
```

**Response:**
```json
{
  "success": true,
  "message": "Post retrieved successfully",
  "post": {
    "id": 1,
    "title": "Sample Post",
    "description": "This is a test post.",
    "tags": ["tech", "blog"],
    "createdAt": "2025-02-10T08:03:19.883Z",
    "createdBy": "jdoe",
    "status": 0,
    "viewCount": 10,
    "isPublished": true
  }
}
```

### 4. Update a Post

**Endpoint:**
```http
PUT /api/posts/{id}
```

**Request Body:**
```json
{
  "title": "Updated Post Title",
  "description": "Updated description.",
  "tags": ["updated", "blog"],
  "isPublished": true
}
```

**Response:**
```json
{
  "success": true,
  "message": "Post updated successfully"
}
```

### 5. Delete a Post

**Endpoint:**
```http
DELETE /api/posts/{id}
```

**Response:**
```json
{
  "success": true,
  "message": "Post deleted successfully"
}
```

---

## Error Handling

All error responses follow this format:
```json
{
  "success": false,
  "message": "Error description"
}
```

## Conclusion

This document provides a structured overview of the **Post API** and **Authentication API**, including endpoints, request and response formats, and logging details. Ensure that API clients handle errors properly and use correct authorization mechanisms.
