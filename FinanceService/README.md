# Finance Service

Currency data and user favorites management microservice.

## Features

- **Currency Data**: Retrieve available currencies and rates
- **User Favorites**: Manage user's favorite currencies
- **Data Persistence**: Store currency and favorite data

## Endpoints

- `GET /api/finance/currencies` - Get all currencies
- `GET /api/finance/favorites` - Get user favorites
- `POST /api/finance/favorites` - Add currency to favorites
- `DELETE /api/finance/favorites/{code}` - Remove from favorites
- `GET /health` - Health check

## Architecture

- **Domain**: Business logic and interfaces
- **Application**: Service layer implementation
- **Infrastructure**: Data access and repositories
- **API**: REST API controllers

## Database

Uses PostgreSQL with Entity Framework Core for currency and favorites data.

## Running

This service runs as part of the main application:

```bash
# Development
docker-compose -f docker-compose.dev.yml up -d

# Production
docker-compose -f docker-compose.prod.yml up -d
```

## Direct Access (Development Only)

In development environment, the service is exposed directly for testing and debugging:

- **Direct Access**: `http://localhost:8082`
- **Health Check**: `http://localhost:8082/health`
- **Swagger UI**: `http://localhost:8082/swagger`

## Production Access

**Note**: In production, this service is not exposed directly. Access via API Gateway at `/api/v1/currencies/*` and `/api/v1/favorites/*` 