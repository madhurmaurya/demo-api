using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductInventoryApi.Models;
using ProductInventoryApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
builder.Services.AddScoped<InventoryService>();

// JWT authentication (placeholder configuration - set in appsettings or environment)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // For production, configure Authority / Audience / TokenValidationParameters with real values or JWKS
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = false,
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
