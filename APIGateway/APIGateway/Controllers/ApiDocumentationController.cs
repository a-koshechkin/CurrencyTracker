using APIGateway.Configuration;
using APIGateway.Constants;
using APIGateway.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace APIGateway.Controllers;

[ApiController]
[Route(ApiConstants.Routes.Base + ApiConstants.Routes.Documentation)]
public class ApiDocumentationController(IOptions<AppSettings> appSettings) : ControllerBase
{
    private readonly ApiSettings _apiSettings = appSettings.Value.Api;

    [HttpGet]
    public IActionResult GetApiDocumentation()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}{_apiSettings.BasePath}";
        
        var response = new ApiDocumentationResponse
        {
            Name = _apiSettings.Name,
            Version = _apiSettings.Version,
            Description = _apiSettings.Description,
            BaseUrl = baseUrl,
            Documentation = "Comprehensive API documentation for the Currency Tracker system",
            Resources = new Dictionary<string, ResourceInfo>
            {
                ["Authentication"] = new ResourceInfo
                {
                    BasePath = ApiEndpoints.Authentication.BasePath,
                    Description = ApiEndpoints.Authentication.Description,
                    Endpoints = ApiEndpoints.Authentication.Endpoints
                },
                ["Currencies"] = new ResourceInfo
                {
                    BasePath = ApiEndpoints.Currencies.BasePath,
                    Description = ApiEndpoints.Currencies.Description,
                    Endpoints = ApiEndpoints.Currencies.Endpoints
                },
                ["Favorites"] = new ResourceInfo
                {
                    BasePath = ApiEndpoints.Favorites.BasePath,
                    Description = ApiEndpoints.Favorites.Description,
                    Endpoints = ApiEndpoints.Favorites.Endpoints
                },
                ["Health"] = new ResourceInfo
                {
                    BasePath = ApiEndpoints.Health.BasePath,
                    Description = ApiEndpoints.Health.Description,
                    Endpoints = ApiEndpoints.Health.Endpoints
                }
            },
            Authentication = new AuthenticationInfo
            {
                Type = ApiEndpoints.AuthenticationInfo.Type,
                Header = ApiEndpoints.AuthenticationInfo.Header,
                Description = ApiEndpoints.AuthenticationInfo.Description,
                TokenFormat = ApiEndpoints.AuthenticationInfo.TokenFormat,
                Expiration = ApiEndpoints.AuthenticationInfo.Expiration
            },
            StatusCodes = new StatusCodesInfo
            {
                Success = ApiEndpoints.StatusCodes.Success,
                ClientError = ApiEndpoints.StatusCodes.ClientError,
                ServerError = ApiEndpoints.StatusCodes.ServerError
            },
            Examples = new Dictionary<string, object>
            {
                ["RegisterUser"] = CreateRegisterUserExample(baseUrl)
            }
        };

        return Ok(response);
    }

    private static object CreateRegisterUserExample(string baseUrl)
    {
        return new
        {
            Request = new
            {
                Method = "POST",
                Url = $"{baseUrl}{ApiEndpoints.Authentication.BasePath}/register",
                Body = new { Name = "john_doe", Password = "securePassword123" }
            },
            Response = new
            {
                Success = true,
                Message = "User registered successfully",
                Data = new
                {
                    UserId = 1,
                    Name = "john_doe",
                    AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                    TokenType = ApiConstants.Authentication.TokenType,
                    ExpiresIn = 3600
                }
            }
        };
    }
} 