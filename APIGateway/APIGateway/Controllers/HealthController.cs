using APIGateway.Configuration;
using APIGateway.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.DTOs.DTOs;
using System.Text.Json;

namespace APIGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(HttpClient httpClient, IOptions<AppSettings> appSettings) : ControllerBase
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ServiceSettings _serviceSettings = appSettings.Value.Services;

    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var healthStatus = new GatewayHealthResponse
        {
            Gateway = new GatewayInfo
            {
                Status = ApiConstants.Status.Healthy,
                Timestamp = DateTime.UtcNow,
                Message = ApiConstants.Messages.GatewayRunning
            },
            Services = []
        };

        var healthTasks = new[]
        {
            CheckServiceHealthAsync("UserService", _serviceSettings.UserService),
            CheckServiceHealthAsync("FinanceService", _serviceSettings.FinanceService)
        };

        var results = await Task.WhenAll(healthTasks);
        
        foreach (var (serviceName, healthInfo) in results)
        {
            healthStatus.Services[serviceName] = healthInfo;
        }

        return Ok(healthStatus);
    }

    private async Task<(string ServiceName, ServiceHealthInfo HealthInfo)> CheckServiceHealthAsync(string serviceName, string serviceUrl)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await _httpClient.GetAsync(serviceUrl, cts.Token);
            
            object? responseContent = null;
            
            if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength > 0)
            {
                var content = await response.Content.ReadAsStringAsync(cts.Token);
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        responseContent = JsonSerializer.Deserialize<object>(content);
                    }
                    catch (JsonException)
                    {
                        responseContent = content;
                    }
                }
            }

            var healthInfo = new ServiceHealthInfo
            {
                Status = response.IsSuccessStatusCode ? ApiConstants.Status.Healthy : ApiConstants.Status.Unhealthy,
                HttpStatus = (int)response.StatusCode,
                Response = responseContent
            };

            return (serviceName, healthInfo);
        }
        catch (Exception ex)
        {
            return (serviceName, new ServiceHealthInfo
            {
                Status = ApiConstants.Status.Unhealthy,
                Error = ex.Message
            });
        }
    }
} 