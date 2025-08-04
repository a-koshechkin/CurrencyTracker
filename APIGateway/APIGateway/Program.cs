using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "cl7F4MnVp24VCH6tkCBCZ9aHJbklPMZnV7jrP7gTO38="))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AuthenticatedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddHttpClient();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Incoming request: {Method} {Path} {QueryString}", 
        context.Request.Method, context.Request.Path, context.Request.QueryString);
    
    await next();
    
    logger.LogInformation("Response: {StatusCode} for {Method} {Path}", 
        context.Response.StatusCode, context.Request.Method, context.Request.Path);
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.MapReverseProxy();

app.MapGet("/", () => new
{
    Message = "Currency Tracker API Gateway",
    Status = "Running",
    Version = "v1",
    Documentation = "/api/v1/docs",
    Health = "/api/health"
});

app.Run();
