# Shared Libraries

Common libraries and components used across all microservices.

## Projects

### Shared.Domain
- **Entities**: Common domain entities (User, Currency, etc.)
- **Attributes**: Custom attributes for data annotations

### Shared.DTOs
- **Data Transfer Objects**: API request/response models
- **Health DTOs**: Health check response models
- **User DTOs**: User-related data models
- **Finance DTOs**: Currency and finance data models

### Shared.Identity
- **JWT Service**: JWT token generation and validation
- **Password Hasher**: Secure password hashing utilities

### Shared.Infrastructure
- **Controllers**: Base controller classes
- **Services**: Common infrastructure services
- **Health Checks**: Database health monitoring
- **Retry Policies**: Polly-based retry configurations

### Shared.GrpcClient
- **gRPC Clients**: Client implementations for service communication
- **Extensions**: gRPC client configuration extensions

### Shared.Protos
- **Protocol Buffers**: gRPC service definitions and contracts

## Usage

Reference these projects in your microservices to share common functionality. 