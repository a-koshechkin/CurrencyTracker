namespace MigrationService.Tool.Constants;

public static class DatabaseConstants
{
    public static class Tables
    {
        public const string User = "user";
        public const string Currency = "currency";
        public const string UserFavorites = "user_favorites";
    }

    public static class Columns
    {
        public static class User
        {
            public const string Id = "id";
            public const string Name = "name";
            public const string Password = "password";
        }

        public static class Currency
        {
            public const string Id = "id";
            public const string Name = "name";
            public const string Rate = "rate";
        }

        public static class UserFavorites
        {
            public const string UserId = "user_id";
            public const string CurrencyId = "currency_id";
        }
    }

    public static class DataTypes
    {
        public const int UserNameMaxLength = 100;
        public const int PasswordMaxLength = 255;
        public const int CurrencyNameMaxLength = 10;
        public const int CurrencyRatePrecision = 10;
        public const int CurrencyRateScale = 4;
    }
} 