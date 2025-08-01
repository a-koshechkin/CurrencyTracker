using Shared.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByNameAsync(string name);
    Task<User> CreateAsync(User user);
} 