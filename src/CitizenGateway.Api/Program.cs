using System.Text;
using CitizenGateway.Api.Auth;
using CitizenGateway.Api.Middleware;
using CitizenGateway.Application;
using CitizenGateway.Infrastructure;
using CitizenGateway.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CitizenGateway API",
        Version = "v1",
        Description = "Örnekköy Belediyesi Vatandaş Servis Entegrasyon Gateway — SENTETİK/DEMO veri. Gerçek kişi verisi yoktur."
    });

    // Swagger'dan Authorize ile Bearer token denemek için.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Örn: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddSingleton<JwtTokenService>();

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // .NET'in claim yeniden adlandırma davranışını kapat — Role claim tutarlı kalsın.
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// Application: use case servisleri (Summary, Request, Auth, Audit, AccessGuard).
builder.Services.AddApplication();

// Infrastructure: EF Core (Npgsql) + repository'ler + Polly'li HttpClient adapter'ları.
// Provider değiştirmek kolay: Infrastructure/DependencyInjection.cs içinde
//   options.UseNpgsql(...) → options.UseSqlServer(...) / UseSqlite(...)
// Connection string ise appsettings.json → ConnectionStrings:GatewayDb (veya env).
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Domain hatalarını HTTP status'e çevir — controller'lar temiz kalsın.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Demo'da Swagger her ortamda açık.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// İlk açılışta migrate + sentetik seed (idempotent).
await DbSeeder.SeedAsync(app.Services);

app.Run();

// WebApplicationFactory entegrasyon testleri için partial Program erişimi.
public partial class Program;
