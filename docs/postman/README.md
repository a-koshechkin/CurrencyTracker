# CurrencyTracker API - Postman Collection

This directory contains Postman collection and environment files for testing the CurrencyTracker API.

## Files

- `CurrencyTracker.postman_collection.json` - Main API collection with all endpoints
- `CurrencyTrackerEnv.dev.json` - Development environment (localhost:5000)
- `CurrencyTrackerEnv.prod.json` - Production environment (localhost:8080)
- `README.md` - This documentation file

## Setup Instructions

### 1. Import Collection and Environment

1. Open Postman
2. Click "Import" button
3. Import all files:
   - `CurrencyTracker.postman_collection.json`
   - `CurrencyTrackerEnv.dev.json`
   - `CurrencyTrackerEnv.prod.json`

### 2. Select Environment

1. In the top-right corner of Postman, select "CurrencyTrackerEnv - Development" environment
2. Verify that the `base_url` variable is set to your API base URL

### 3. Authentication Flow

1. **Register** a new user (optional)
2. **Login** with existing credentials
3. The JWT token will be automatically saved to the environment
4. All subsequent requests will use the saved token

## Environment Variables

| Variable | Description | Default Value |
|----------|-------------|---------------|
| `base_url` | Base URL for the server | `http://localhost:5000` |
| `base_api` | Base URL for the API (v1 endpoints) | `{{base_url}}/api/v1/` |
| `currencyCode` | Currency code for favorites operations | `CAD` |
| `existing_name` | Username for login | `john_doe` |
| `existing_password` | Password for login | `password123` |
| `new_name` | Username for registration | `john_doe1` |
| `new_password` | Password for registration | `password123` |
| `token` | JWT authentication token | (auto-populated after login) |

## API Endpoints

### Authentication
- **POST** `/auth/login` - User login (uses `{{existing_name}}` and `{{existing_password}}`)
- **POST** `/auth/register` - User registration (uses `{{new_name}}` and `{{new_password}}`)

### Currency Management
- **GET** `/currencies` - Get all currencies with rates

### User Favorites
- **GET** `/favorites` - Get user's favorite currencies
- **POST** `/favorites` - Add currency to favorites (uses `{{currencyCode}}`)
- **DELETE** `/favorites/{currencyCode}` - Remove currency from favorites (uses `{{currencyCode}}`)

### System
- **GET** `/docs` - API documentation (Swagger) - uses `/api/v1/docs`
- **GET** `/health` - Health check - uses `/api/health` (non-versioned)

## Testing Workflow

1. **Start with Health Check** - Verify API is running
2. **Login** - Authenticate and get token
3. **Get Currencies** - Fetch available currencies
4. **Manage Favorites** - Add/remove favorite currencies
5. **Check Documentation** - Access Swagger docs

## Tips

- The login request automatically saves the JWT token to environment variables
- All authenticated requests use the `{{token}}` variable
- Use the external API endpoint to test the Russian Central Bank integration
- Check the console for token saving confirmation messages
- Change `currencyCode` variable to test different currencies
- Update credential variables to test with different users

## Troubleshooting

### Common Issues

1. **401 Unauthorized**: Make sure you've logged in and the token is saved
2. **404 Not Found**: Verify the `base_api` URL is correct
3. **Connection Refused**: Ensure the API server is running
4. **Token Not Saving**: Check that your API response contains `accessToken` in the data object
5. **Environment Not Selected**: Make sure you have an environment selected in Postman

## Environment Management

### Switching Between Environments

You have multiple environment files for different deployment stages:

1. **Import All Environment Files**:
   - Import all `.json` environment files into Postman
   - Each will appear as a separate environment

2. **Switch Environments**:
   - Click the environment dropdown in the top-right corner
   - Select the desired environment:
     - `CurrencyTrackerEnv - Development` (localhost:5000)
     - `CurrencyTrackerEnv - Production` (localhost:8080)

3. **Environment-Specific Variables**:
   - Each environment has its own `base_api` URL
   - Tokens are environment-specific (login again when switching)
   - Variables are automatically updated when switching

### Environment URLs

| Environment | Base URL | Use Case |
|-------------|----------|----------|
| Development | `http://localhost:5000` | Local development |
| Production | `http://localhost:8080` | Production server |

### Environment Setup

To create custom environments:
1. Copy any existing environment file
2. Update the `base_url` value (only this needs to change!)
3. Import the new environment file
4. The `base_api` will automatically update since it references `{{base_url}}`

**Benefits of this approach:**
- Only need to change one variable (`base_url`) when switching environments
- `base_api` automatically updates to the correct versioned path
- Reduces configuration errors and maintenance overhead

## Collection Features

- **Automatic Token Management**: Login automatically saves JWT tokens
- **Organized Structure**: Endpoints grouped by functionality
- **Descriptions**: Each request includes helpful descriptions
- **Environment Variables**: Flexible configuration for different environments
- **Variable References**: `base_api` automatically references `{{base_url}}/api/v1/`
- **Security**: Passwords and tokens marked as secret variables 