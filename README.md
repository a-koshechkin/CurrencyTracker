# Currency Tracker

A microservices-based currency tracking application built with .NET 8.

## Architecture

- **APIGateway**: Reverse proxy and API documentation
- **UserService**: User authentication and management
- **FinanceService**: Currency data and favorites management
- **CurrencyWorker**: Background service for currency updates
- **MigrationService**: Database migration tool
- **Shared**: Common libraries and DTOs

## Quick Start

### Development Environment

```bash
# Start all services with development configuration
docker-compose -f docker-compose.dev.yml up -d
```

### Production Environment

```bash
# Start all services with production configuration
docker-compose -f docker-compose.prod.yml up -d
```

### What happens at start:
- Database (PostgreSQL) is started
- MigrationService runs database migrations
- All microservices start after migrations complete
- CurrencyWorker begins updating currency rates every 60 minutes

## Services

### Development Environment
- **API Gateway**: `http://localhost:5000`
- **UserService**: `http://localhost:8081` (direct access)
- **FinanceService**: `http://localhost:8082` (direct access)
- **PostgreSQL**: `localhost:5432` (direct access for development)
- **API Documentation**: `http://localhost:5000/api/v1/docs`
- **Health Check**: `http://localhost:5000/api/health`

### Production Environment
- **API Gateway**: `http://localhost:8080`
- **API Documentation**: `http://localhost:8080/api/v1/docs`
- **Health Check**: `http://localhost:8080/api/health`

**Note**: In development, individual services and database are exposed for direct testing and debugging. In production, only the API Gateway is exposed for security.

## Tech Stack

- .NET 8
- PostgreSQL
- Docker
- JWT Authentication
- YARP Reverse Proxy

## API Testing

For API testing and documentation, see the [Postman Collection](./docs/postman/README.md) which includes:
- Complete API endpoint collection
- Environment configurations for development and production
- Automatic JWT token management
- Comprehensive testing workflows 