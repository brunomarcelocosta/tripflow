using System.Diagnostics;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IUserContext userContext)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        var userId = userContext.IdentityProviderUserId ?? "anonymous";

        using (logger.BeginScope(new Dictionary<string, object?>
        {
            ["TraceId"] = traceId,
            ["UserId"] = userId
        }))
        {
            logger.LogInformation(
                "Request {Method} {Path} - TraceId: {TraceId}, UserId: {UserId}",
                context.Request.Method,
                context.Request.Path,
                traceId,
                userId);

            await next(context);
        }
    }
}
