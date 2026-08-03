using CitizenGateway.Application;
using CitizenGateway.Infrastructure;
using CitizenGateway.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application: use case servisleri (Summary, Request, Auth, Audit, AccessGuard).
builder.Services.AddApplication();

// Infrastructure: EF Core (Npgsql) + repository'ler + Polly'li HttpClient adapter'ları.
// Provider değiştirmek kolay: Infrastructure/DependencyInjection.cs içinde
//   options.UseNpgsql(...) → options.UseSqlServer(...) / UseSqlite(...)
// Connection string ise appsettings.json → ConnectionStrings:GatewayDb (veya env).
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Geliştirmede Swagger açık; production'da da demo için açık bırakılabilir (Phase 6'da netleşir).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// İlk açılışta migrate + sentetik seed (idempotent).
await DbSeeder.SeedAsync(app.Services);

app.Run();

// WebApplicationFactory entegrasyon testleri için partial Program erişimi (Phase 9).
public partial class Program;
