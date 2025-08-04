using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace APIGateway.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration);
        
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
        var corsSettings = configuration.GetSection("Cors").Get<CorsSettings>() ?? new CorsSettings();

        services.AddJwtAuthentication(jwtSettings);
        services.AddCorsPolicy(corsSettings);
        
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings? jwtSettings)
    {
        if (jwtSettings == null)
            throw new InvalidOperationException("JWT settings are required");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

        return services;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services, CorsSettings corsSettings)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(corsSettings.PolicyName, policy =>
            {
                if (corsSettings.AllowAnyOrigin)
                    policy.AllowAnyOrigin();
                if (corsSettings.AllowAnyMethod)
                    policy.AllowAnyMethod();
                if (corsSettings.AllowAnyHeader)
                    policy.AllowAnyHeader();
            });
        });

        return services;
    }
} 