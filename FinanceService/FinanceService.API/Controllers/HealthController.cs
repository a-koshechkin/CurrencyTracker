using Microsoft.AspNetCore.Authorization;
using Shared.Infrastructure.Controllers;

namespace FinanceService.API.Controllers;

[AllowAnonymous]
public class HealthController : HealthControllerBase
{
    protected override string ServiceName => "Finance Service";
} 