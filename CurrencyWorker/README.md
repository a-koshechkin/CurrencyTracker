# Currency Worker

A .NET 8 background service that fetches currency exchange rates from the Russian Central Bank API and updates the database.

## Features

- ✅ **Russian Central Bank Integration** - Fetches rates from `http://www.cbr.ru/scripts/XML_daily.asp`
- ✅ **Background Service** - Runs continuously with configurable intervals
- ✅ **Simple Database Updates** - Updates existing rates and adds new currencies
- ✅ **Clean Architecture** - Well-structured, maintainable code
- ✅ **Docker Support** - Containerized deployment
- ✅ **Resilience** - Error handling and retry logic

## Architecture

```
CurrencyWorker/
├── CurrencyWorker.Domain/           # Domain entities and interfaces
├── CurrencyWorker.Application/      # Business logic services
├── CurrencyWorker.Infrastructure/   # External API and database access
├── CurrencyWorker.Worker/          # Background service and DI setup
└── docker-compose.yml              # Docker orchestration
```

## Quick Start

### Using Docker Compose

```bash
cd CurrencyWorker
docker-compose up
```

This will:
1. Start PostgreSQL 15 container
2. Wait for database to be healthy
3. Start CurrencyWorker background service
4. Begin fetching and updating currency rates every hour

### Manual Setup

1. **Ensure database is running** with the currency table created
2. **Update connection string** in `appsettings.json`
3. **Run the worker**:
   ```bash
   dotnet run --project CurrencyWorker.Worker
   ```

## Configuration

### Update Interval
```json
{
  "CurrencyUpdate": {
    "IntervalMinutes": 60  // Update every hour
  }
}
```

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=currencytracker;Username=postgres;Password=admin;Port=5432"
  }
}
```

## API Integration

The service fetches data from the Russian Central Bank XML API:
- **URL**: `http://www.cbr.ru/scripts/XML_daily.asp`
- **Format**: XML
- **Data**: Currency codes, names, and exchange rates to RUB

### Sample XML Response
```xml
<ValCurs Date="31.07.2025" name="Foreign Currency Market">
    <Valute ID="R01010">
        <NumCode>036</NumCode>
        <CharCode>AUD</CharCode>
        <Nominal>1</Nominal>
        <Name>Австралийский доллар</Name>
        <Value>53,2826</Value>
        <VunitRate>53,2826</VunitRate>
    </Valute>
</ValCurs>
```

## Database Updates

The service implements a simple and effective currency management strategy:

### **Update Logic:**
1. **Update Existing Currencies** - Updates rates for currencies already in the database (matched by name)
2. **Add New Currencies** - Inserts new currencies that exist in the API but not in the database

### **Database Schema:**
```sql
currency table:
- id (primary key)
- name (currency name)
- rate (exchange rate to RUB)
```

### **Simple and Reliable:**
- **Name-based matching** using ILIKE for flexibility
- **Upsert approach** - update if exists, insert if new
- **No complex state management** - keeps it simple and maintainable

## Logging

The service provides detailed logging:
- **Info**: Service start/stop, successful updates, new currencies added
- **Warning**: API failures, no data received
- **Error**: Database errors, parsing failures
- **Debug**: Individual currency updates

## Development

### Project Structure

#### Domain Layer
- `CurrencyRate` - Domain entity
- `ICurrencyApiService` - API contract
- `ICurrencyRepository` - Data access contract

#### Application Layer
- `CurrencyUpdateService` - Business logic orchestration

#### Infrastructure Layer
- `RussianCentralBankApiService` - XML API integration
- `CurrencyRepository` - Database operations with simple upsert logic

#### Worker Layer
- `CurrencyUpdateWorker` - Background service
- `Program.cs` - DI configuration

### Adding New Features

1. **New API Source**: Implement `ICurrencyApiService`
2. **New Database**: Implement `ICurrencyRepository`
3. **New Logic**: Add services to Application layer

## Troubleshooting

### Common Issues

1. **API Connection Failed**: Check network connectivity to `cbr.ru`
2. **Database Connection Failed**: Verify connection string and PostgreSQL status
3. **No Updates**: Check if currency names match between API and database

### Logs

Enable debug logging for detailed information:
```json
{
  "Logging": {
    "LogLevel": {
      "CurrencyWorker": "Debug"
    }
  }
}
```

## Monitoring

The service logs:
- **Start/Stop events**
- **API fetch results**
- **Database update counts** (updated, inserted)
- **Error details**

Monitor logs to ensure:
- Regular updates are occurring
- API responses are valid
- Database updates are successful
- New currencies are being added

## Production Deployment

1. **Update connection strings** for production database
2. **Configure logging** for production environment
3. **Set appropriate update intervals** (consider API rate limits)
4. **Monitor service health** and logs
5. **Set up alerts** for failures

## License

This project is part of the CurrencyTracker microservices architecture. 