namespace FinanceService.Domain.Interfaces;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(int userId);
} 