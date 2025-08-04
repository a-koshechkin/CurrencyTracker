using APIGateway.Configuration;
using APIGateway.Constants;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppConfiguration(builder.Configuration);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsProduction())
{
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
    builder.Logging.AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Warning);
    builder.Logging.AddFilter("Yarp", LogLevel.Warning);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

builder.Services.AddControllers();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ApiConstants.Policies.AuthenticatedUser, policy =>
    {
        policy.RequireAuthenticatedUser();
    });

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHealthChecks();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Incoming request: {Method} {Path}", 
            context.Request.Method, context.Request.Path);
        
        await next();
        
        logger.LogInformation("Response: {StatusCode} for {Method} {Path}", 
            context.Response.StatusCode, context.Request.Method, context.Request.Path);
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors(ApiConstants.Policies.AllowAll);
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks(ApiConstants.Routes.Health);
app.MapControllers();
app.MapReverseProxy();

var appSettings = app.Services.GetRequiredService<IOptions<AppSettings>>().Value;
app.MapGet("/", () => new
{
    Message = ApiConstants.Messages.ApiGatewayMessage,
    Status = ApiConstants.Status.Running,
    Version = appSettings.Api.Version,
    Documentation = appSettings.Api.DocumentationPath,
    Health = appSettings.Api.HealthPath
});

app.Run();
