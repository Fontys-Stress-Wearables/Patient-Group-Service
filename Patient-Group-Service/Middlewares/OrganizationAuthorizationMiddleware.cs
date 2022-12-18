using Microsoft.Identity.Web;
using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;

namespace Patient_Group_Service.Middlewares;

public class OrganizationAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public OrganizationAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IOrganizationService organizationService)
    {
        var tenant = context.User.GetTenantId();

        if (tenant == null)
        {
            throw new NotFoundException("User has no tenantId");
        }

        if (!organizationService.Exists(tenant))
        {
            throw new UnauthorizedException($"tenant not found with the ID: {tenant}");
        }

        await _next(context);
    }
}

public static class OrganizationAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseOrganizationAuthorizationMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<OrganizationAuthorizationMiddleware>();
    }
}