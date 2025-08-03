# Migration Service

A .NET 8 console application for managing database migrations using FluentMigrator and PostgreSQL.

## Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- PostgreSQL 15 (or use Docker)

## Quick Start

### Using Docker Compose (Recommended)

```bash
cd MigrationService
docker-compose up
```

This will:
1. Start PostgreSQL 15 container
2. Wait for database to be healthy
3. Run all pending migrations
4. Seed initial currency data

## Usage

### Available Commands
- `--migrate` - Run all pending migrations
- `--rollback --version=X` - Rollback to specific version

## Database Schema

The service creates the following tables:

### `user` Table
- `id` (int, primary key, identity)
- `name` (varchar(100), not null)
- `password` (varchar(255), not null)

### `currency` Table
- `id` (int, primary key, identity)
- `name` (varchar(100), not null)
- `rate` (decimal, not null) - Exchange rate to RUB

### `user_favorites` Table
- `user_id` (int, foreign key to user.id)
- `currency_id` (int, foreign key to currency.id)
- Composite primary key (user_id, currency_id)

## Configuration

### Polly Retry Settings
```json
{
  "Polly": {
    "DatabaseRetry": {
      "MaxRetries": 10,
      "BaseDelaySeconds": 2
    },
    "MigrationRetry": {
      "MaxRetries": 3,
      "BaseDelaySeconds": 2
    }
  }
}
```

### Logging
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "FluentMigrator.Runner": "Information",
      "MigrationService.Tool.Services": "Debug"
    }
  }
}
```

## Project Structure

```
MigrationService/
├── MigrationService.Tool/           # Main console application
│   ├── Configuration/              # DI configuration
│   ├── Migrations/                 # FluentMigrator migrations
│   ├── Services/                   # Business logic services
│   ├── Program.cs                  # Entry point
│   └── appsettings.json           # Configuration
├── docker-compose.yml             # Docker orchestration
└── README.md                      # This file
```


## Troubleshooting

### Common Issues

1. **Connection refused**: Ensure PostgreSQL is running and accessible
2. **Permission denied**: Check database user permissions
3. **Migration already applied**: Use rollback to revert changes

### Logs

Enable debug logging to see detailed migration information:
```json
{
  "Logging": {
    "LogLevel": {
      "MigrationService.Tool.Services": "Debug"
    }
  }
}
```