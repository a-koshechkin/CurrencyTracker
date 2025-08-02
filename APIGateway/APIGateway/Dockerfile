# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file first
COPY ["APIGateway/APIGateway.sln", "APIGateway/"]

# Copy APIGateway project files
COPY ["APIGateway/APIGateway/APIGateway.csproj", "APIGateway/APIGateway/"]

# Copy shared projects
COPY ["Shared/Shared.Domain/Shared.Domain.csproj", "Shared/Shared.Domain/"]
COPY ["Shared/Shared.DTOs/Shared.DTOs.csproj", "Shared/Shared.DTOs/"]
COPY ["Shared/Shared.Identity/Shared.Identity.csproj", "Shared/Shared.Identity/"]
COPY ["Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "Shared/Shared.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "APIGateway/APIGateway/APIGateway.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/APIGateway/APIGateway"
RUN dotnet build "APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.dll"] 