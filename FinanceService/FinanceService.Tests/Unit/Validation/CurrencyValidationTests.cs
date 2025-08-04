using FinanceService.Tests.TestHelpers;
using Shared.Domain.Entities;
using Shared.DTOs.DTOs;

namespace FinanceService.Tests.Unit.Validation;

public class CurrencyValidationTests
{
    #region Currency Entity Validation Tests

    [Fact]
    public void Currency_WithValidData_IsValid()
    {
        // Arrange & Act
        var currency = TestDataBuilder.CreateTestCurrency(id: 1, name: "USD", rate: 1.0m);

        // Assert
        Assert.Equal(1, currency.Id);
        Assert.Equal("USD", currency.Name);
        Assert.Equal(1.0m, currency.Rate);
        Assert.NotNull(currency.UserFavorites);
        Assert.Empty(currency.UserFavorites);
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    public void Currency_WithValidCurrencyCodes_IsValid(string validCurrencyCode)
    {
        // Arrange & Act
        var currency = TestDataBuilder.CreateTestCurrency(name: validCurrencyCode, rate: 1.0m);

        // Assert
        Assert.Equal(validCurrencyCode, currency.Name);
        Assert.Equal(1.0m, currency.Rate);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(100.0)]
    public void Currency_WithValidRates_IsValid(double validRate)
    {
        // Arrange & Act
        var currency = TestDataBuilder.CreateTestCurrency(rate: (decimal)validRate);

        // Assert
        Assert.Equal((decimal)validRate, currency.Rate);
    }

    #endregion

    #region CurrencyRate DTO Validation Tests

    [Fact]
    public void CurrencyRate_WithValidData_IsValid()
    {
        // Arrange & Act
        var currencyRate = new CurrencyRate(
            CurrencyCode: "USD",
            Rate: 1.0m
        );

        // Assert
        Assert.Equal("USD", currencyRate.CurrencyCode);
        Assert.Equal(1.0m, currencyRate.Rate);
    }

    #endregion

    #region DTO Validation Tests (Essential Only)

    [Fact]
    public void CurrencyResponse_WithValidData_IsValid()
    {
        // Arrange & Act
        var currencyResponse = new CurrencyResponse
        {
            Name = "USD",
            Rate = 1.0m
        };

        // Assert
        Assert.Equal("USD", currencyResponse.Name);
        Assert.Equal(1.0m, currencyResponse.Rate);
    }

    [Fact]
    public void FavoriteCurrencyInfo_WithValidData_IsValid()
    {
        // Arrange & Act
        var favoriteCurrencyInfo = new FavoriteCurrencyInfo
        {
            CurrencyName = "USD",
            Rate = 1.0m
        };

        // Assert
        Assert.Equal("USD", favoriteCurrencyInfo.CurrencyName);
        Assert.Equal(1.0m, favoriteCurrencyInfo.Rate);
    }

    [Fact]
    public void AddFavoriteRequest_WithValidData_IsValid()
    {
        // Arrange & Act
        var addFavoriteRequest = new AddFavoriteRequest
        {
            CurrencyCode = "USD"
        };

        // Assert
        Assert.Equal("USD", addFavoriteRequest.CurrencyCode);
    }

    [Fact]
    public void UserFavoritesResponse_WithValidData_IsValid()
    {
        // Arrange & Act
        var userFavoritesResponse = new UserFavoritesResponse
        {
            FavoriteCurrencies = new List<FavoriteCurrencyInfo>
            {
                new() { CurrencyName = "USD", Rate = 1.0m },
                new() { CurrencyName = "EUR", Rate = 0.85m }
            }
        };

        // Assert
        Assert.NotNull(userFavoritesResponse.FavoriteCurrencies);
        Assert.Equal(2, userFavoritesResponse.FavoriteCurrencies.Count);
        Assert.Equal("USD", userFavoritesResponse.FavoriteCurrencies[0].CurrencyName);
        Assert.Equal(1.0m, userFavoritesResponse.FavoriteCurrencies[0].Rate);
        Assert.Equal("EUR", userFavoritesResponse.FavoriteCurrencies[1].CurrencyName);
        Assert.Equal(0.85m, userFavoritesResponse.FavoriteCurrencies[1].Rate);
    }

    [Fact]
    public void UserFavoritesResponse_WithEmptyList_IsValid()
    {
        // Arrange & Act
        var userFavoritesResponse = new UserFavoritesResponse
        {
            FavoriteCurrencies = new List<FavoriteCurrencyInfo>()
        };

        // Assert
        Assert.NotNull(userFavoritesResponse.FavoriteCurrencies);
        Assert.Empty(userFavoritesResponse.FavoriteCurrencies);
    }

    #endregion
} 