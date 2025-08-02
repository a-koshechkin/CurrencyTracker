using FinanceService.Domain.Interfaces;
using Shared.DTOs.DTOs;

namespace FinanceService.Application.Services;

public class FinanceService(IUserRepository userRepository, IUserFavoriteRepository userFavoriteRepository, ICurrencyRepository currencyRepository) : IFinanceService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserFavoriteRepository _userFavoriteRepository = userFavoriteRepository;
    private readonly ICurrencyRepository _currencyRepository = currencyRepository;

    public async Task<UserFavoritesResponse> GetUserFavoritesAsync(int userId)
    {
        var userExists = await _userRepository.UserExistsAsync(userId);
        if (!userExists)
        {
            throw new ArgumentException($"User with ID {userId} not found");
        }

        var userFavorites = await _userFavoriteRepository.GetUserFavoritesAsync(userId);
        
        if (!userFavorites.Any())
        {
            return new UserFavoritesResponse
            {
                FavoriteCurrencies = []
            };
        }

        var currencyIds = userFavorites.Select(uf => uf.CurrencyId).Distinct();
        
        var currencies = await _currencyRepository.GetCurrenciesByIdsAsync(currencyIds);
        
        var favoriteCurrencies = currencies.Select(c => new FavoriteCurrencyInfo
        {
            CurrencyName = c.Name,
            Rate = c.Rate
        }).ToList();

        return new UserFavoritesResponse
        {
            FavoriteCurrencies = favoriteCurrencies
        };
    }

    public async Task<List<CurrencyResponse>> GetAllCurrenciesAsync()
    {
        var currencies = await _currencyRepository.GetAllCurrenciesAsync();
        
        return [.. currencies.Select(c => new CurrencyResponse
        {
            Name = c.Name,
            Rate = c.Rate
        })];
    }

    public async Task<bool> AddToFavoritesAsync(int userId, string currencyCode)
    {
        var userExists = await _userRepository.UserExistsAsync(userId);
        if (!userExists)
        {
            throw new ArgumentException($"User with ID {userId} not found");
        }

        var currency = await _currencyRepository.GetCurrencyByNameAsync(currencyCode) ?? throw new ArgumentException($"Currency with code {currencyCode} not found");
        var existingFavorite = await _userFavoriteRepository.GetUserFavoriteAsync(userId, currency.Id);
        if (existingFavorite != null)
        {
            return false;
        }

        await _userFavoriteRepository.AddUserFavoriteAsync(userId, currency.Id);
        return true;
    }

    public async Task<bool> RemoveFromFavoritesAsync(int userId, string currencyCode)
    {
        var userExists = await _userRepository.UserExistsAsync(userId);
        if (!userExists)
        {
            throw new ArgumentException($"User with ID {userId} not found");
        }

        var currency = await _currencyRepository.GetCurrencyByNameAsync(currencyCode) ?? throw new ArgumentException($"Currency with code {currencyCode} not found");
        var existingFavorite = await _userFavoriteRepository.GetUserFavoriteAsync(userId, currency.Id);
        if (existingFavorite == null)
        {
            return false;
        }

        await _userFavoriteRepository.RemoveUserFavoriteAsync(userId, currency.Id);
        return true;
    }
} 