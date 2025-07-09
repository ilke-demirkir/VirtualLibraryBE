using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
namespace VirtualLibraryAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var isDev = _env.IsDevelopment();

            var response = new
            {
                message = isDev ? ex.Message : "An unexpected error occured",
                code = "SERVER_ERROR",
                stackTrace = isDev ? ex.StackTrace : null,
                inner = isDev ? ex.InnerException?.Message : null
            };
               

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}