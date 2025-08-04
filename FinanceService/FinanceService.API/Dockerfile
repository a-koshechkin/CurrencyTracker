FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FinanceService/FinanceService.API/FinanceService.API.csproj", "FinanceService/FinanceService.API/"]
COPY ["FinanceService/FinanceService.Application/FinanceService.Application.csproj", "FinanceService/FinanceService.Application/"]
COPY ["FinanceService/FinanceService.Domain/FinanceService.Domain.csproj", "FinanceService/FinanceService.Domain/"]
COPY ["FinanceService/FinanceService.Infrastructure/FinanceService.Infrastructure.csproj", "FinanceService/FinanceService.Infrastructure/"]
COPY ["Shared/Shared.Domain/Shared.Domain.csproj", "Shared/Shared.Domain/"]
COPY ["Shared/Shared.DTOs/Shared.DTOs.csproj", "Shared/Shared.DTOs/"]

RUN dotnet restore "FinanceService/FinanceService.API/FinanceService.API.csproj"

COPY . .
WORKDIR "/src/FinanceService/FinanceService.API"

RUN dotnet build "FinanceService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinanceService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinanceService.API.dll"] 