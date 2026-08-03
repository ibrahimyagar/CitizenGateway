using CitizenGateway.Application.Abstractions;
using CitizenGateway.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CitizenGateway.Application;

/// <summary>
/// Application servis kayıtları — Api composition root'tan tek satırla eklenir.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CitizenSummaryService>();
        services.AddScoped<CitizenDirectoryService>();
        services.AddScoped<ServiceRequestService>();
        services.AddScoped<AuthService>();
        services.AddScoped<CitizenAccessGuard>();
        services.AddScoped<AuditQueryService>();
        services.AddScoped<IAuditLogger, AuditLogger>();

        return services;
    }
}
