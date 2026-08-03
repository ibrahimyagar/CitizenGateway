using CitizenGateway.Application.Features.Audit;
using CitizenGateway.Application.Features.Auth;
using CitizenGateway.Application.Features.Citizens;
using CitizenGateway.Application.Features.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace CitizenGateway.Application;

/// <summary>
/// Application use-case kayıtları — Api composition root'tan tek satırla eklenir.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICitizenSummaryService, CitizenSummaryService>();
        services.AddScoped<ICitizenDirectoryService, CitizenDirectoryService>();
        services.AddScoped<ICitizenAccessGuard, CitizenAccessGuard>();
        services.AddScoped<IServiceRequestService, ServiceRequestService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuditQueryService, AuditQueryService>();
        services.AddScoped<IAuditLogger, AuditLogger>();

        return services;
    }
}
