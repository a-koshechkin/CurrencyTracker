# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy NuGet packages first
COPY ["nuget-packages/", "nuget-packages/"]

# Copy CurrencyWorker projects
COPY ["CurrencyWorker/CurrencyWorker.Worker/CurrencyWorker.Worker.csproj", "CurrencyWorker/CurrencyWorker.Worker/"]
COPY ["CurrencyWorker/CurrencyWorker.Application/CurrencyWorker.Application.csproj", "CurrencyWorker/CurrencyWorker.Application/"]
COPY ["CurrencyWorker/CurrencyWorker.Domain/CurrencyWorker.Domain.csproj", "CurrencyWorker/CurrencyWorker.Domain/"]
COPY ["CurrencyWorker/CurrencyWorker.Infrastructure/CurrencyWorker.Infrastructure.csproj", "CurrencyWorker/CurrencyWorker.Infrastructure/"]

# Add local NuGet source and restore dependencies
RUN dotnet nuget add source "/src/nuget-packages" --name LocalPackages
RUN dotnet restore "CurrencyWorker/CurrencyWorker.Worker/CurrencyWorker.Worker.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/CurrencyWorker/CurrencyWorker.Worker"
RUN dotnet build "CurrencyWorker.Worker.csproj" -c Release -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CurrencyWorker.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CurrencyWorker.Worker.dll"]