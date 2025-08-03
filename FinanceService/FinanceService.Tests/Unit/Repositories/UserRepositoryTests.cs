using FinanceService.Infrastructure;
using FinanceService.Infrastructure.Repositories;
using FinanceService.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Tests.Unit.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly DbContextOptions<FinanceDbContext> _options;
    private readonly FinanceDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new FinanceDbContext(_options);
        _repository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region UserExistsAsync Tests

    [Fact]
    public async Task UserExistsAsync_ExistingUser_ReturnsTrue()
    {
        // Arrange
        var testUser = TestDataBuilder.CreateTestUser(1);
        await _context.Users.AddAsync(testUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UserExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UserExistsAsync_NonExistingUser_ReturnsFalse()
    {
        // Arrange
        var testUser = TestDataBuilder.CreateTestUser(1);
        await _context.Users.AddAsync(testUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UserExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UserExistsAsync_EmptyDatabase_ReturnsFalse()
    {
        // Act
        var result = await _repository.UserExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task UserExistsAsync_InvalidUserId_ReturnsFalse(int userId)
    {
        // Arrange
        var testUser = TestDataBuilder.CreateTestUser(1);
        await _context.Users.AddAsync(testUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UserExistsAsync(userId);

        // Assert
        Assert.False(result);
    }

    #endregion
} 