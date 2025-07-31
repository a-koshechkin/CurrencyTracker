FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget-packages/", "nuget-packages/"]
COPY ["MigrationService/MigrationService.Tool/MigrationService.Tool.csproj", "MigrationService/MigrationService.Tool/"]
RUN dotnet nuget add source "/src/nuget-packages" --name LocalPackages
RUN dotnet restore "./MigrationService/MigrationService.Tool/MigrationService.Tool.csproj"
COPY . .
WORKDIR "/src/MigrationService/MigrationService.Tool"
RUN dotnet build "./MigrationService.Tool.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MigrationService.Tool.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MigrationService.Tool.dll"]