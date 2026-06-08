using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ProductInventoryApi.Controllers;

[ApiController]
[Route("dev")]
public class DevTokenController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public DevTokenController(IWebHostEnvironment env, IConfiguration config)
    {
        _env = env;
        _config = config;
    }

    [HttpPost("token")]
    public IActionResult Token([FromBody] DevTokenRequest req)
    {
        if (!_env.IsDevelopment()) return NotFound();

        var key = _config["Jwt:DevSigningKey"] ?? _config["DevSigningKey"];
        if (string.IsNullOrEmpty(key)) return BadRequest(new { error = "Dev signing key not configured (Jwt:DevSigningKey)" });

        var subject = string.IsNullOrEmpty(req.Sub) ? "dev-user" : req.Sub;
        var scopes = req.Scopes != null && req.Scopes.Length > 0 ? string.Join(' ', req.Scopes) : "inventory:read";

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, subject),
            new Claim("scope", scopes)
        };

        // Audience is set on the token itself; no need to add as a claim here.

        using var sha = System.Security.Cryptography.SHA256.Create();
        var keyBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(req.ExpiresHours > 0 ? req.ExpiresHours : 1);
        var token = new JwtSecurityToken(
            issuer: null,
            audience: req.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { access_token = tokenString, token_type = "Bearer", expires_in = (int)(expires - DateTime.UtcNow).TotalSeconds });
    }
}

public record DevTokenRequest
(
    string? Sub,
    string[]? Scopes,
    string? Audience,
    int ExpiresHours
);
