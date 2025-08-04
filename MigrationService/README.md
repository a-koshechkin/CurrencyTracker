# Migration Service

Database migration tool using FluentMigrator.

## Features

- **Database Migrations**: Schema and data migrations
- **Rollback Support**: Version-based rollbacks
- **PostgreSQL Support**: Native PostgreSQL integration
- **Seed Data**: Initial data population

## Usage

```bash
# Run all pending migrations
dotnet run --migrate

# Rollback to specific version
dotnet run --rollback --version=003
```

## Migrations

- `001_CreateUsersTable.cs` - User table schema
- `002_CreateCurrenciesTable.cs` - Currency table schema
- `003_CreateUserFavouritesTable.cs` - Favorites table schema
- `004_SeedInitialData.cs` - Initial data seeding

## Configuration

Database connection configured in `appsettings.json`.

## Running

```bash
# Build and run with Docker
docker build -f MigrationService/Dockerfile -t migration-service .

# Run migrations
docker run --rm --network currency-tracker-network \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Database=currency_tracker;Username=postgres;Password=postgres" \
  migration-service --migrate

# Or run locally
dotnet run --project MigrationService.Tool
```