using Shared.Infrastructure.Controllers;

namespace UserService.Api.Controllers;

public class HealthController : HealthControllerBase
{
    protected override string ServiceName => "User Service";
} 