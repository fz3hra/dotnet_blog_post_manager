# API Usage Documentation

## Introduction

This document provides a structured overview of the **Auth API** and **Post API** for user authentication and managing posts. It includes endpoint details, request and response formats, required headers, and potential errors.

## Dependencies

This API uses the following dependencies:
- `AutoMapper` for object mapping
- `Entity Framework Core` for database operations
- `ILogger` for logging
- `System.IdentityModel.Tokens.Jwt` for token handling
- `Microsoft.AspNetCore.Identity` for user management

## Authorization and Headers

All API endpoints require the following headers:

- `Authorization`: `Bearer <token>` - A valid Bearer token for authentication (for secured endpoints).
- `Content-Type`: `application/json` - Indicates that the request body is in JSON format.

---

## Auth API Endpoints

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
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string",
  "userName": "string"
}
```

**Response:**
- **200 OK:**
```json
{
  "message": "Registration successful."
}
```
- **400 Bad Request:**
```json
{
  "errors": {
    "email": "Email is already registered."
  }
}
```
- **500 Internal Server Error:**
```json
{
  "title": "An error occurred while processing request",
  "status": 500,
  "detail": "Error message"
}
```

---

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
  "email": "string",
  "password": "string"
}
```

**Response:**
- **200 OK:**
```json
{
  "userId": "string",
  "token": "string"
}
```
- **401 Unauthorized:**
```json
{
  "message": "Invalid credentials."
}
```
- **500 Internal Server Error:**
```json
{
  "title": "An error occurred while processing request",
  "status": 500,
  "detail": "Error message"
}
```

---

## Post API Endpoints

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
  "title": "string",
  "description": "string",
  "tags": ["string"],
  "featuredImageUrl": "string",
  "excerpt": "string",
  "isPublished": true
}
```

**Response:**
- **200 OK:**
```json
{
  "success": true,
  "message": "Post created successfully",
  "post": {
    "title": "string",
    "description": "string",
    "tags": ["string"],
    "featuredImageUrl": "string",
    "excerpt": "string",
    "id": 0,
    "createdAt": "2025-02-10T08:03:19.883Z",
    "createdBy": "string",
    "status": 0,
    "viewCount": 0,
    "isPublished": true,
    "lastModifiedAt": "2025-02-10T08:03:19.883Z",
    "lastModifiedBy": "string"
  }
}
```
- **401 Unauthorized:**
```json
{
  "success": false,
  "message": "User not authenticated"
}
```
- **500 Internal Server Error:**
```json
{
  "success": false,
  "message": "An error occurred while creating the post."
}
```

---

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

**Query Parameters:**
- `status` (optional): Integer (0, 1, 2)

**Response:**
- **200 OK:**
```json
{
  "success": true,
  "message": "Posts retrieved successfully.",
  "posts": [
    {
      "title": "string",
      "description": "string",
      "tags": ["string"],
      "featuredImageUrl": "string",
      "excerpt": "string",
      "id": 0,
      "createdAt": "2025-02-10T08:04:14.195Z",
      "createdBy": "string",
      "status": 0,
      "viewCount": 0,
      "isPublished": true,
      "lastModifiedAt": "2025-02-10T08:04:14.195Z",
      "lastModifiedBy": "string"
    }
  ]
}
```
- **401 Unauthorized:**
```json
{
  "success": false,
  "message": "User not authenticated"
}
```
- **500 Internal Server Error:**
```json
{
  "success": false,
  "message": "An error occurred while retrieving posts."
}
```

---

### 3. Get Post by ID

**Endpoint:**
```http
GET /api/posts/{id}
```

**Headers:**
```json
{
  "Authorization": "Bearer <token>",
  "Content-Type": "application/json"
}
```

**Path Parameters:**
- `id` (required): Integer, the unique identifier of the post.

**Response:**
- **200 OK:**
```json
{
  "success": true,
  "message": "Post retrieved successfully.",
  "post": {
    "title": "string",
    "description": "string",
    "tags": ["string"],
    "featuredImageUrl": "string",
    "excerpt": "string",
    "id": 0,
    "createdAt": "2025-02-10T08:04:35.814Z",
    "createdBy": "string",
    "status": 0,
    "viewCount": 0,
    "isPublished": true,
    "lastModifiedAt": "2025-02-10T08:04:35.814Z",
    "lastModifiedBy": "string"
  }
}
```
- **404 Not Found:**
```json
{
  "success": false,
  "message": "Post not found."
}
```
- **500 Internal Server Error:**
```json
{
  "success": false,
  "message": "An error occurred while retrieving the post."
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

## Logging and Debugging

- Errors and exceptions are logged using `ILogger`.
- Logs can be retrieved from the application logs for debugging issues.

## Conclusion

This document provides a structured overview of the Auth and Post APIs, including endpoints, request and response formats, and error handling. Ensure that API clients handle errors properly and use correct authorization mechanisms.
