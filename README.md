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

```bash
# Build migration image
docker build -f MigrationService/Dockerfile -t currencytracker-migration .

# Run migrations
docker run --rm --network currency-tracker-network \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Database=currency_tracker;Username=postgres;Password=postgres" \
  currencytracker-migration --migrate

# Start services
docker-compose up -d
```

## Services

- **API Gateway**: `http://localhost:5000`
- **API Documentation**: `http://localhost:5000/api/v1/docs`

## Tech Stack

- .NET 8
- PostgreSQL
- Docker
- JWT Authentication
- YARP Reverse Proxy 