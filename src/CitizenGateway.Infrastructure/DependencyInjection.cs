using CitizenGateway.Application.Abstractions;
using CitizenGateway.Application.Options;
using CitizenGateway.Infrastructure.Http;
using CitizenGateway.Infrastructure.Persistence;
using CitizenGateway.Infrastructure.Persistence.Repositories;
using CitizenGateway.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CitizenGateway.Infrastructure;

/// <summary>
/// Infrastructure kayıt noktası — Api yalnızca bu extension'ı çağırır (composition root sade kalır).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Provider değişimi: UseNpgsql → UseSqlServer / UseSqlite tek satır.
        // Connection string appsettings / env / docker-compose ile gelir.
        services.AddDbContext<GatewayDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("GatewayDb")
                ?? "Host=localhost;Port=5432;Database=citizen_gateway;Username=postgres;Password=postgres";

            options.UseNpgsql(connectionString);
        });

        services.Configure<MockServicesOptions>(configuration.GetSection(MockServicesOptions.SectionName));
        var mockOptions = configuration.GetSection(MockServicesOptions.SectionName).Get<MockServicesOptions>()
            ?? new MockServicesOptions();

        services.AddScoped<ICitizenRepository, CitizenRepository>();
        services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IExternalServiceClient, ExternalServiceClient>();
        services.AddSingleton<IPasswordVerifier, IdentityPasswordVerifier>();

        // Named HttpClient + Polly: her mock servis bağımsız circuit breaker state tutar.
        AddResilientClient(services, ExternalServiceClient.SporTesisiClientName, mockOptions.SporTesisiBaseUrl);
        AddResilientClient(services, ExternalServiceClient.KutuphaneClientName, mockOptions.KutuphaneBaseUrl);
        AddResilientClient(services, ExternalServiceClient.CozumMerkeziClientName, mockOptions.CozumMerkeziBaseUrl);

        return services;
    }

    private static void AddResilientClient(IServiceCollection services, string name, string baseUrl)
    {
        services.AddHttpClient(name, client =>
            {
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddPolicyHandler(ResiliencePolicies.GetRetryPolicy())
            .AddPolicyHandler(ResiliencePolicies.GetCircuitBreakerPolicy());
    }
}
