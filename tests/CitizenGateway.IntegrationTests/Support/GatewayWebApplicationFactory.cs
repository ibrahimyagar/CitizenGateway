using CitizenGateway.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CitizenGateway.IntegrationTests.Support;

/// <summary>
/// Gerçek HTTP pipeline + izole PostgreSQL connection string.
/// IExternalServiceClient stub'lanır; auth/DB/middleware gerçek kalır.
/// </summary>
public sealed class GatewayWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public GatewayWebApplicationFactory(string connectionString) =>
        _connectionString = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.UseSetting("ConnectionStrings:GatewayDb", _connectionString);
        builder.UseSetting("Jwt:SigningKey", "Integration-Test-Signing-Key-At-Least-32-Chars!");
        builder.UseSetting("Jwt:Issuer", "CitizenGateway");
        builder.UseSetting("Jwt:Audience", "CitizenGateway.Clients");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IExternalServiceClient>();
            services.AddSingleton<IExternalServiceClient, StubExternalServiceClient>();
        });
    }
}
