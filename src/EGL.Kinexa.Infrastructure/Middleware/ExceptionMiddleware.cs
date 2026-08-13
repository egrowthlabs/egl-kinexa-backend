using System.Net;
using System.Text.Json;
using EGL.Kinexa.Domain.Exceptions;
using EGL.Kinexa.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EGL.Kinexa.Infrastructure.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        int statusCode = (int)HttpStatusCode.InternalServerError;
        var response = ApiResponse<object>.Fail("An internal server error occurred.");

        switch (exception)
        {
            case NotFoundException notFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                response = ApiResponse<object>.Fail(notFoundException.Message);
                break;
            case BusinessRuleException businessRuleException:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = ApiResponse<object>.Fail(businessRuleException.Message);
                break;
            case ConflictException conflictException:
                statusCode = (int)HttpStatusCode.Conflict;
                response = ApiResponse<object>.Fail(conflictException.Message);
                break;
            case UnauthorizedException unauthorizedException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                response = ApiResponse<object>.Fail(unauthorizedException.Message);
                break;
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                var errors = string.Join(" ", validationException.Errors.Select(e => e.ErrorMessage));
                response = ApiResponse<object>.Fail($"Validation Failed. {errors}");
                break;
        }

        context.Response.StatusCode = statusCode;
        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(result);
    }
}
