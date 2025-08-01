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