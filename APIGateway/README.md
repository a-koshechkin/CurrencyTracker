# API Gateway

Reverse proxy and API documentation service for the Currency Tracker application.

## Features

- **Reverse Proxy**: Routes requests to appropriate microservices using YARP
- **API Documentation**: Interactive API documentation endpoint
- **Authentication**: JWT token validation and forwarding
- **Health Monitoring**: Aggregated health checks from all services

## Endpoints

- `/api/v1/docs` - API documentation
- `/api/health` - Health status
- `/api/v1/*` - Proxied to microservices

## Configuration

Routes are configured in `appsettings.json` under the `ReverseProxy` section.

## Running

This service runs as part of the main application:

```bash
# Start all services (recommended)
docker-compose up -d
``` 