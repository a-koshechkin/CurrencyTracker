static class ApiDocumentation
{
    public static readonly object RootResponse = new
    {
        Message = "Currency Tracker API Gateway",
        Status = "Running",
        Version = "v1",
        Documentation = "RESTful API following REST principles",
        Routes = new[]
        {
            "/api/v1/auth/* → User Authentication (UserService)",
            "/api/v1/currencies/* → Currency Data (FinanceService)",
            "/api/v1/favorites/* → User Favorites (FinanceService)"
        },
        Examples = new
        {
            Authentication = new[]
            {
                "POST /api/v1/auth/register",
                "POST /api/v1/auth/login",
                "POST /api/v1/auth/logout"
            },
            Currencies = new[]
            {
                "GET /api/v1/currencies"
            },
            Favorites = new[]
            {
                "GET /api/v1/favorites",
                "POST /api/v1/favorites",
                "DELETE /api/v1/favorites/{id}"
            }
        }
    };
}
