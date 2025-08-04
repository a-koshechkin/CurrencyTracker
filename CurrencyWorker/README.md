# Currency Worker

Background service for currency data updates.

## Features

- **Currency Updates**: Periodic currency rate updates from external APIs
- **Data Synchronization**: Keeps currency data current
- **Background Processing**: Runs as a Windows Service or container

## External APIs

- **Russian Central Bank API**: Source for currency rates (`http://www.cbr.ru/scripts/XML_daily.asp`)

## Architecture

- **Domain**: Business logic and interfaces
- **Application**: Service layer implementation
- **Infrastructure**: External API clients and data access
- **Worker**: Background service implementation

## Configuration

Update intervals and API endpoints configured in `appsettings.json`.

## Running

This service runs as part of the main application:

```bash
# Development
docker-compose -f docker-compose.dev.yml up -d

# Production
docker-compose -f docker-compose.prod.yml up -d
```

**Note**: This is a background service that runs automatically and updates currency rates every 60 minutes. 