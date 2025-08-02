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
COPY ["UserService/UserService.sln", "UserService/"]

# Copy UserService project files
COPY ["UserService/UserService.Api/UserService.Api.csproj", "UserService/UserService.Api/"]
COPY ["UserService/UserService.Application/UserService.Application.csproj", "UserService/UserService.Application/"]
COPY ["UserService/UserService.Domain/UserService.Domain.csproj", "UserService/UserService.Domain/"]
COPY ["UserService/UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService/UserService.Infrastructure/"]

# Copy shared projects
COPY ["Shared/Shared.Domain/Shared.Domain.csproj", "Shared/Shared.Domain/"]
COPY ["Shared/Shared.DTOs/Shared.DTOs.csproj", "Shared/Shared.DTOs/"]
COPY ["Shared/Shared.Identity/Shared.Identity.csproj", "Shared/Shared.Identity/"]

# Restore dependencies
RUN dotnet restore "UserService/UserService.Api/UserService.Api.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/UserService/UserService.Api"
RUN dotnet build "UserService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.Api.dll"] 