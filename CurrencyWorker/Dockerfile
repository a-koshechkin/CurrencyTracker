FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CurrencyWorker/CurrencyWorker.Worker/CurrencyWorker.Worker.csproj", "CurrencyWorker/CurrencyWorker.Worker/"]
COPY ["CurrencyWorker/CurrencyWorker.Application/CurrencyWorker.Application.csproj", "CurrencyWorker/CurrencyWorker.Application/"]
COPY ["CurrencyWorker/CurrencyWorker.Domain/CurrencyWorker.Domain.csproj", "CurrencyWorker/CurrencyWorker.Domain/"]
COPY ["CurrencyWorker/CurrencyWorker.Infrastructure/CurrencyWorker.Infrastructure.csproj", "CurrencyWorker/CurrencyWorker.Infrastructure/"]
COPY ["Shared/Shared.Domain/Shared.Domain.csproj", "Shared/Shared.Domain/"]
COPY ["Shared/Shared.Identity/Shared.Identity.csproj", "Shared/Shared.Identity/"]

RUN dotnet restore "CurrencyWorker/CurrencyWorker.Worker/CurrencyWorker.Worker.csproj"
COPY . .
WORKDIR "/src/CurrencyWorker/CurrencyWorker.Worker"
RUN dotnet build "CurrencyWorker.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CurrencyWorker.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CurrencyWorker.Worker.dll"] 