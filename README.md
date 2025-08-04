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

- **API Gateway**: `http://localhost:5000` (dev) / `http://localhost:8080` (prod)
- **API Documentation**: `http://localhost:5000/api/v1/docs` (dev) / `http://localhost:8080/api/v1/docs` (prod)

## Tech Stack

- .NET 8
- PostgreSQL
- Docker
- JWT Authentication
- YARP Reverse Proxy 