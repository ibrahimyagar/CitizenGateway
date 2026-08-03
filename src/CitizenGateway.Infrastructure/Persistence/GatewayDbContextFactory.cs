using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CitizenGateway.Infrastructure.Persistence;

/// <summary>
/// `dotnet ef migrations` design-time factory — Api ayağa kalkmadan migration üretir.
/// </summary>
public sealed class GatewayDbContextFactory : IDesignTimeDbContextFactory<GatewayDbContext>
{
    public GatewayDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GatewayDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=citizen_gateway;Username=postgres;Password=postgres");

        return new GatewayDbContext(optionsBuilder.Options);
    }
}
