using CitizenGateway.Api.Auth;
using CitizenGateway.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CitizenGateway.Api.Filters;

/// <summary>
/// /summary ve /requests erişimlerini otomatik audit'ler.
/// Neden action filter? Middleware tüm pipeline'ı görür ama route tcNo + JWT claim
/// action bağlamında daha doğal; IAuditLogger Application'da test edilebilir kalır.
/// </summary>
public sealed class AuditCitizenAccessFilter : IAsyncActionFilter
{
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<AuditCitizenAccessFilter> _logger;

    public AuditCitizenAccessFilter(IAuditLogger auditLogger, ILogger<AuditCitizenAccessFilter> logger)
    {
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();

        var http = context.HttpContext;
        if (http.User.Identity?.IsAuthenticated != true)
            return;

        if (!context.RouteData.Values.TryGetValue("tcNo", out var tcValue))
            return;

        var tcNo = tcValue?.ToString();
        if (string.IsNullOrWhiteSpace(tcNo))
            return;

        try
        {
            var userId = http.User.GetUserId();
            var role = http.User.GetUserRole();
            var endpoint = $"{http.Request.Method} {http.Request.Path}";
            var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            await _auditLogger.LogAccessAsync(
                userId,
                role,
                tcNo,
                endpoint,
                ip,
                http.RequestAborted);
        }
        catch (Exception ex)
        {
            // Audit yazılamasa bile asıl API yanıtı bozulmamalı.
            _logger.LogError(ex, "Audit kaydı yazılamadı. Path={Path}", http.Request.Path);
        }
    }
}

/// <summary>Controller/action'a yapıştırılan işaret — DI ile filter örneği üretir.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuditCitizenAccessAttribute : TypeFilterAttribute
{
    public AuditCitizenAccessAttribute() : base(typeof(AuditCitizenAccessFilter))
    {
    }
}
