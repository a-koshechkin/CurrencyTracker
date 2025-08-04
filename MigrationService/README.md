# Migration Service

Database migration tool using FluentMigrator.

## Features

- **Database Migrations**: Schema and data migrations
- **Rollback Support**: Version-based rollbacks
- **PostgreSQL Support**: Native PostgreSQL integration
- **Seed Data**: Initial data population (included in migrations)
- **Automatic Execution**: Runs automatically at container startup

## Migrations

- `001_CreateUsersTable.cs` - User table schema
- `002_CreateCurrenciesTable.cs` - Currency table schema
- `003_CreateUserFavouritesTable.cs` - Favorites table schema
- `004_SeedInitialData.cs` - Initial data seeding (currencies, users, favorites)

## Configuration

Database connection configured in `appsettings.json`.

## Running

### Normal Usage (Automatic)
Migrations run automatically when the container starts:

```bash
# Development
docker-compose -f docker-compose.dev.yml up -d

# Production
docker-compose -f docker-compose.prod.yml up -d
```

### Additional Operations (Manual)
For running migrations or rollback operations, you can run the already deployed container again:

```bash
# Run all pending migrations (including seed data)
docker run --rm --network currency-tracker-network \
  -e ConnectionStrings__DefaultConnection="Host=postgresql;Database=currencytracker_dev;Username=postgres;Password=admin" \
  currencytracker-migration --migrate

# Rollback to specific version
docker run --rm --network currency-tracker-network \
  -e ConnectionStrings__DefaultConnection="Host=postgresql;Database=currencytracker_dev;Username=postgres;Password=admin" \
  currencytracker-migration --rollback --version=003
```

### Development Only (.NET CLI)
```bash
# Run locally
dotnet run --project MigrationService.Tool --migrate

# Rollback to specific version
dotnet run --project MigrationService.Tool --rollback --version=003
```

**Note**: In production, migrations are handled automatically by the MigrationService container at startup.