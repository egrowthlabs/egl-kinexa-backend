using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace EGL.Kinexa.Infrastructure.Middleware;

public class SingleSessionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public SingleSessionMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? context.User.FindFirstValue("id");
            
            var currentToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(currentToken))
            {
                if (_cache.TryGetValue($"UserSession_{userId}", out string? storedToken))
                {
                    if (storedToken != currentToken)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Session expired. Logged in from another device.");
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
