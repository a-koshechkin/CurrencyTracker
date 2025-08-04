namespace Shared.DTOs.DTOs;

public record UserFavoritesResponse
{
    public List<FavoriteCurrencyInfo> FavoriteCurrencies { get; init; } = [];
}

public record FavoriteCurrencyInfo
{
    public string CurrencyName { get; init; } = string.Empty;
    public decimal Rate { get; init; }
}

public record CurrencyResponse
{
    public string Name { get; init; } = string.Empty;
    public decimal Rate { get; init; }
}

public record AddFavoriteRequest
{
    public string CurrencyCode { get; init; } = string.Empty;
}

public record CurrencyRate(string CurrencyCode, decimal Rate); 