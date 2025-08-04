using APIGateway.Configuration;
using APIGateway.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace APIGateway.Controllers;

[ApiController]
[Route(ApiConstants.Routes.Base)]
public class ApiController(IOptions<AppSettings> appSettings) : ControllerBase
{
    private readonly ApiSettings _apiSettings = appSettings.Value.Api;

    [HttpGet]
    public IActionResult GetApiInfo()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}{_apiSettings.BasePath}";
        
        return Ok(new
        {
            Name = _apiSettings.Name,
            Version = _apiSettings.Version,
            Description = _apiSettings.Description,
            BaseUrl = baseUrl,
            Documentation = $"{Request.Scheme}://{Request.Host}{_apiSettings.DocumentationPath}",
            Health = $"{Request.Scheme}://{Request.Host}{_apiSettings.HealthPath}",
            Resources = GetResources(),
            Authentication = $"{ApiConstants.Authentication.Bearer} Token required for protected endpoints"
        });
    }

    private static string[] GetResources()
    {
        return
        [
            $"{ApiEndpoints.Authentication.BasePath}/* - {ApiEndpoints.Authentication.Description}",
            $"{ApiEndpoints.Currencies.BasePath}/* - {ApiEndpoints.Currencies.Description}",
            $"{ApiEndpoints.Favorites.BasePath}/* - {ApiEndpoints.Favorites.Description}"
        ];
    }
} 