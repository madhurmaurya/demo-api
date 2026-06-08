using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductInventoryApi.Models;
using ProductInventoryApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Product Inventory API", Version = "v1" });
    // Add Bearer auth definition
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer <token>'"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});
builder.Services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
builder.Services.AddScoped<InventoryService>();

// JWT authentication (configure Authority/Audience or JWKS URI in appsettings or environment)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var cfg = builder.Configuration.GetSection("Jwt");
    var authority = cfg["Authority"];
    var audience = cfg["Audience"];
    var jwksUri = cfg["JwksUri"];
    var devSigningKey = builder.Configuration["Jwt:DevSigningKey"] ?? builder.Configuration["DevSigningKey"];

    options.RequireHttpsMetadata = true;

    // In Development, prefer a configured dev signing key to validate locally minted tokens
    if (builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(devSigningKey))
    {
        var sha = System.Security.Cryptography.SHA256.Create();
        var keyBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(devSigningKey));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = !string.IsNullOrEmpty(audience),
            ValidAudience = !string.IsNullOrEmpty(audience) ? audience : null,
            ValidateLifetime = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes)
        };
    }
    else if (!string.IsNullOrEmpty(authority))
    {
        // Use a direct JWKS URI (not OIDC). Resolve signing keys on demand and cache.
        // Prefer standard OIDC authority which exposes JWKS via metadata
        options.Authority = authority;
        if (!string.IsNullOrEmpty(audience)) options.Audience = audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = !string.IsNullOrEmpty(audience),
            ValidateLifetime = true
        };
    }
    else if (!string.IsNullOrEmpty(jwksUri))
    {
        // Use a direct JWKS URI (not OIDC). Resolve signing keys on demand and cache.
        if (!string.IsNullOrEmpty(audience)) options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = true, ValidAudience = audience, ValidateLifetime = true };
        else options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false, ValidateLifetime = true };

        var http = new HttpClient();
        var jwksCache = new Dictionary<string, Microsoft.IdentityModel.Tokens.SecurityKey>();

        options.TokenValidationParameters.IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
            // simple per-process cache
            if (!string.IsNullOrEmpty(kid) && jwksCache.TryGetValue(kid, out var key)) return new[] { key };

            var resp = http.GetStringAsync(jwksUri).GetAwaiter().GetResult();
            var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(resp);
            var keys = jwks.Keys.Select(k => (Microsoft.IdentityModel.Tokens.SecurityKey)k).ToList();
            foreach (var k in jwks.Keys) if (!string.IsNullOrEmpty(k.Kid)) jwksCache[k.Kid] = k;
            return keys;
        };
    }
    else
    {
        // Development fallback: accept unsigned tokens but validate lifetime
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = false,
            ValidateLifetime = true
        };
    }
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Inventory API v1"));
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
