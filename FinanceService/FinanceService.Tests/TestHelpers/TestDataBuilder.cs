using Shared.Domain.Entities;

namespace FinanceService.Tests.TestHelpers;

public static class TestDataBuilder
{
    private static int _userCounter = 0;
    private static int _currencyCounter = 0;

    #region Data Generation Methods

    private static string GenerateUniqueUsername()
    {
        return $"user_{Interlocked.Increment(ref _userCounter)}_{Guid.NewGuid():N}"[..20];
    }

    private static string GenerateUniqueCurrencyName()
    {
        return $"Currency_{Interlocked.Increment(ref _currencyCounter)}_{Guid.NewGuid():N}"[..30];
    }

    #endregion

    #region Entity Builders

    public static User CreateTestUser(int? id = null, string? name = null)
    {
        return new User
        {
            Id = id ?? Random.Shared.Next(1, 1000),
            Name = name ?? GenerateUniqueUsername(),
            Password = "hashed_password"
        };
    }

    public static Currency CreateTestCurrency(int? id = null, string? name = null, decimal? rate = null)
    {
        return new Currency
        {
            Id = id ?? Random.Shared.Next(1, 1000),
            Name = name ?? GenerateUniqueCurrencyName(),
            Rate = rate ?? Random.Shared.Next(100, 10000) / 100m
        };
    }

    public static UserFavorite CreateTestUserFavorite(int userId, int currencyId)
    {
        return new UserFavorite
        {
            UserId = userId,
            CurrencyId = currencyId
        };
    }

    public static List<Currency> CreateTestCurrencies(int count = 10)
    {
        var currencies = new List<Currency>();
        for (int i = 0; i < count; i++)
        {
            currencies.Add(CreateTestCurrency());
        }
        return currencies;
    }

    public static List<UserFavorite> CreateTestUserFavorites(int userId, List<int> currencyIds)
    {
        var favorites = new List<UserFavorite>();
        foreach (var currencyId in currencyIds)
        {
            favorites.Add(CreateTestUserFavorite(userId, currencyId));
        }
        return favorites;
    }

    #endregion
} 