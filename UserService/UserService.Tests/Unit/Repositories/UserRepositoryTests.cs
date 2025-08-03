using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;

namespace UserService.Tests.Unit.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly UserDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetByNameAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User { Name = "testuser", Password = "hashed_password" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Name);
        Assert.Equal("hashed_password", result.Password);
    }

    [Fact]
    public async Task GetByNameAsync_NonExistingUser_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByNameAsync("nonexistentuser");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidUser_ReturnsCreatedUser()
    {
        // Arrange
        var user = new User { Name = "newuser", Password = "hashed_password" };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal("newuser", result.Name);
        Assert.Equal("hashed_password", result.Password);

        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == "newuser");
        Assert.NotNull(savedUser);
        Assert.Equal(result.Id, savedUser.Id);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
} 