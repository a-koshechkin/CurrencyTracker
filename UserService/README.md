# User Service

User authentication and management microservice.

## Features

- **User Registration**: Create new user accounts
- **User Login**: JWT token-based authentication
- **User Logout**: Token invalidation
- **Password Hashing**: Secure password storage

## Endpoints

- `POST /api/user/register` - Register new user
- `POST /api/user/login` - User login
- `POST /api/user/logout` - User logout
- `GET /health` - Health check

## Architecture

- **Domain**: Business logic and interfaces
- **Application**: Service layer implementation
- **Infrastructure**: Data access and external services
- **API**: REST API controllers

## Database

Uses PostgreSQL with Entity Framework Core for user data storage.

## Running

This service runs as part of the main application:

```bash
# Development
docker-compose -f docker-compose.dev.yml up -d

# Production
docker-compose -f docker-compose.prod.yml up -d

# Or run individually (development only)
docker-compose -f docker-compose.dev.yml up userservice
```

## Direct Access (Development Only)

In development environment, the service is exposed directly for testing and debugging:

- **Direct Access**: `http://localhost:8081`
- **Health Check**: `http://localhost:8081/health`
- **Swagger UI**: `http://localhost:8081/swagger`

## Production Access

**Note**: In production, this service is not exposed directly. Access via API Gateway at `/api/v1/auth/*` 