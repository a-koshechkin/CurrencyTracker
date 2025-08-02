using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APIGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(HttpClient httpClient, IConfiguration configuration) : ControllerBase
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;

    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var healthStatus = new
        {
            Gateway = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Message = "API Gateway is running"
            },
            Services = new Dictionary<string, object>()
        };

        try
        {
            var userServiceUrl = _configuration["Services:UserService"] ?? "http://user-service:5000/health";
            var userResponse = await _httpClient.GetAsync(userServiceUrl);
            var userContent = await userResponse.Content.ReadAsStringAsync();
            
            healthStatus.Services["UserService"] = new
            {
                Status = userResponse.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                Response = JsonSerializer.Deserialize<object>(userContent),
                HttpStatus = (int)userResponse.StatusCode
            };
        }
        catch (Exception ex)
        {
            healthStatus.Services["UserService"] = new
            {
                Status = "Unhealthy",
                Error = ex.Message
            };
        }

        try
        {
            var financeServiceUrl = _configuration["Services:FinanceService"] ?? "http://finance-service:5000/health";
            var financeResponse = await _httpClient.GetAsync(financeServiceUrl);
            var financeContent = await financeResponse.Content.ReadAsStringAsync();
            
            healthStatus.Services["FinanceService"] = new
            {
                Status = financeResponse.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                Response = JsonSerializer.Deserialize<object>(financeContent),
                HttpStatus = (int)financeResponse.StatusCode
            };
        }
        catch (Exception ex)
        {
            healthStatus.Services["FinanceService"] = new
            {
                Status = "Unhealthy",
                Error = ex.Message
            };
        }

        return Ok(healthStatus);
    }
} 