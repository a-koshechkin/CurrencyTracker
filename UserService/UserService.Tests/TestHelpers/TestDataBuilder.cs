namespace UserService.Tests.TestHelpers;

public static class TestDataBuilder
{
    private static int _userCounter = 0;

    #region Data Generation Methods

    private static string GenerateUniqueUsername()
    {
        return $"user_{Interlocked.Increment(ref _userCounter)}_{Guid.NewGuid():N}"[..20];
    }

    private static string GenerateSecurePassword()
    {
        return $"Password{Guid.NewGuid():N}"[..20];
    }

    #endregion

    #region Test Data Collections

    public static IEnumerable<object[]> GetEmptyOrNullInputData()
    {
        return
        [
            ["", GenerateSecurePassword()],
            [GenerateUniqueUsername(), ""],
            [null!, GenerateSecurePassword()],
            [GenerateUniqueUsername(), null!],
            ["   ", GenerateSecurePassword()],
            [GenerateUniqueUsername(), "   "]
        ];
    }

    public static IEnumerable<object[]> GetControlCharactersInputData()
    {
        return
        [
            ["user" + (char)0 + "name", GenerateSecurePassword()],
            ["user" + (char)1 + "name", GenerateSecurePassword()]
        ];
    }

    public static IEnumerable<object[]> GetValidLengthInputData()
    {
        return
        [
            ["a", GenerateSecurePassword()],
            ["a".PadRight(100, 'a'), GenerateSecurePassword()],
            [GenerateUniqueUsername(), "a".PadRight(255, 'a')]
        ];
    }

    public static IEnumerable<object[]> GetTooLongInputData()
    {
        return
        [
            ["a".PadRight(101, 'a'), GenerateSecurePassword()],
            [GenerateUniqueUsername(), "a".PadRight(256, 'a')]
        ];
    }

    #endregion
} 