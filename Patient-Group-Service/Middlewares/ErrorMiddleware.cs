using System.Net;
using System.Text.Json;
using Patient_Group_Service.Exceptions;

namespace Patient_Group_Service.Middlewares;

public class ErrorMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            if (error is AppException applicationError)
            {
                response.StatusCode = (int)applicationError.StatusCode;
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var result = JsonSerializer.Serialize(new { message = error.Message });
            await response.WriteAsync(result);
        }
    }
}
