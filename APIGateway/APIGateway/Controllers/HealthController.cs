using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;
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
        var healthStatus = new GatewayHealthResponse
        {
            Gateway = new GatewayInfo
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Message = "API Gateway is running"
            },
            Services = []
        };

        try
        {
            var userServiceUrl = _configuration["Services:UserService"] ?? "http://user-service:5000/health";
            var userResponse = await _httpClient.GetAsync(userServiceUrl);
            var userContent = await userResponse.Content.ReadAsStringAsync();
            
            healthStatus.Services["UserService"] = new ServiceHealthInfo
            {
                Status = userResponse.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                Response = JsonSerializer.Deserialize<object>(userContent),
                HttpStatus = (int)userResponse.StatusCode
            };
        }
        catch (Exception ex)
        {
            healthStatus.Services["UserService"] = new ServiceHealthInfo
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
            
            healthStatus.Services["FinanceService"] = new ServiceHealthInfo
            {
                Status = financeResponse.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                Response = JsonSerializer.Deserialize<object>(financeContent),
                HttpStatus = (int)financeResponse.StatusCode
            };
        }
        catch (Exception ex)
        {
            healthStatus.Services["FinanceService"] = new ServiceHealthInfo
            {
                Status = "Unhealthy",
                Error = ex.Message
            };
        }

        return Ok(healthStatus);
    }
} 