using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ProductInventoryApi.Middleware;

public class JwtScopeMiddleware
{
    private readonly RequestDelegate _next;

    public JwtScopeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Placeholder: additional centralized scope checks could be added here
        await _next(context);
    }
}
