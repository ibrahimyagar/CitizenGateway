using Bogus;
using CitizenGateway.Domain.Entities;
using CitizenGateway.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MockServices.Shared;

namespace CitizenGateway.Infrastructure.Persistence;

/// <summary>
/// SENTETİK seed — gerçek kişi/belediye verisi YOKTUR.
/// SharedCitizenCatalog (seed=42) ile mock servisler aynı TC havuzunu paylaşır.
/// </summary>
public static class DbSeeder
{
    public const string PersonelUsername = "personel";
    public const string PersonelPassword = "Personel123!";
    public const string VatandasUsername = "vatandas";
    public const string VatandasPassword = "Vatandas123!";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        // Migrate: geliştirmede şemayı otomatik güncel tutar (demo kolaylığı).
        await db.Database.MigrateAsync(cancellationToken);

        if (await db.Citizens.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Seed atlandı — veritabanında zaten vatandaş kaydı var.");
            return;
        }

        logger.LogInformation("Sentetik seed başlıyor (SharedCitizenCatalog, {Count} kayıt)...", SharedCitizenCatalog.CitizenCount);

        Randomizer.Seed = new Random(SharedCitizenCatalog.Seed);
        var faker = new Faker("tr");

        var citizens = SharedCitizenCatalog.All
            .Select(c => Citizen.Create(
                c.TcNo,
                c.AdSoyad,
                DateOnly.FromDateTime(faker.Date.Between(new DateTime(1960, 1, 1), new DateTime(2005, 12, 31))),
                faker.Phone.PhoneNumber("05#########")))
            .ToList();

        await db.Citizens.AddRangeAsync(citizens, cancellationToken);

        // Birkaç örnek talep — talep akışı demosu için.
        var sampleRequests = new List<ServiceRequest>
        {
            ServiceRequest.Create(citizens[0].Id, RequestType.KursKaydi, TargetService.SporTesisi, RequestStatus.Onaylandi),
            ServiceRequest.Create(citizens[1].Id, RequestType.RandevuTalebi, TargetService.Kutuphane, RequestStatus.Beklemede),
            ServiceRequest.Create(citizens[2].Id, RequestType.SikayetAcma, TargetService.CozumMerkezi, RequestStatus.Reddedildi),
            ServiceRequest.Create(citizens[0].Id, RequestType.RandevuTalebi, TargetService.Kutuphane, RequestStatus.Beklemede)
        };
        await db.ServiceRequests.AddRangeAsync(sampleRequests, cancellationToken);

        // PasswordHasher: düz metin asla saklanmaz; Identity hasher demo için yeterli.
        var hasher = new PasswordHasher<ApplicationUser>();
        var hashProbe = ApplicationUser.CreatePersonel("probe", "probe-hash-placeholder");

        var personel = ApplicationUser.CreatePersonel(
            PersonelUsername,
            hasher.HashPassword(hashProbe, PersonelPassword));

        var linkedCitizen = citizens[0];
        var vatandas = ApplicationUser.CreateVatandas(
            VatandasUsername,
            hasher.HashPassword(hashProbe, VatandasPassword),
            linkedCitizen.Id);

        await db.Users.AddRangeAsync([personel, vatandas], cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seed tamamlandı. Demo: {Personel}/{PersonelPass}, {Vatandas}/{VatandasPass} (bağlı TC: {Tc})",
            PersonelUsername,
            PersonelPassword,
            VatandasUsername,
            VatandasPassword,
            linkedCitizen.TcNo);
    }
}
