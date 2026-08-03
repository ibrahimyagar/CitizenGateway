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
/// </summary>
public static class DbSeeder
{
    public const string PersonelEmail = "aylin.kara@ornekkoy.bel.tr";
    public const string PersonelDisplayName = "Aylin Kara";
    public const string PersonelPassword = "Personel123!";

    /// <summary>Ayşe Demir (katalog[0]) — giriş kimliği kendi TC'sidir.</summary>
    public const string VatandasPassword = "Vatandas123!";

    /// <summary>Test / dokümantasyon — personel giriş e-postası.</summary>
    public const string PersonelUsername = PersonelEmail;

    /// <summary>Test — demo vatandaşın T.C. (katalog[0], Ayşe Demir).</summary>
    public static string VatandasUsername => SharedCitizenCatalog.All[0].TcNo;

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        await db.Database.MigrateAsync(cancellationToken);

        if (await db.Citizens.AnyAsync(cancellationToken))
        {
            await EnsureModernLoginAccountsAsync(db, logger, cancellationToken);
            logger.LogInformation("Seed atlandı — vatandaşlar mevcut; giriş hesapları senkronlandı.");
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

        var sampleRequests = new List<ServiceRequest>
        {
            ServiceRequest.Create(citizens[0].Id, RequestType.KursKaydi, TargetService.SporTesisi, RequestStatus.Onaylandi),
            ServiceRequest.Create(citizens[0].Id, RequestType.RandevuTalebi, TargetService.Kutuphane, RequestStatus.Beklemede),
            ServiceRequest.Create(citizens[1].Id, RequestType.RandevuTalebi, TargetService.Kutuphane, RequestStatus.Beklemede),
            ServiceRequest.Create(citizens[2].Id, RequestType.SikayetAcma, TargetService.CozumMerkezi, RequestStatus.Reddedildi)
        };
        await db.ServiceRequests.AddRangeAsync(sampleRequests, cancellationToken);

        var hasher = new PasswordHasher<ApplicationUser>();
        var probe = ApplicationUser.CreatePersonel("probe@ornekkoy.bel.tr", "Probe", "probe-hash-placeholder");

        var personel = ApplicationUser.CreatePersonel(
            PersonelEmail,
            PersonelDisplayName,
            hasher.HashPassword(probe, PersonelPassword));

        var linkedCitizen = citizens[0];
        var vatandas = ApplicationUser.CreateVatandas(
            linkedCitizen.TcNo,
            linkedCitizen.AdSoyad,
            hasher.HashPassword(probe, VatandasPassword),
            linkedCitizen.Id);

        await db.Users.AddRangeAsync([personel, vatandas], cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seed tamamlandı. Personel: {Email}, Vatandaş TC: {Tc}",
            PersonelEmail,
            linkedCitizen.TcNo);
    }

    /// <summary>Eski personel/vatandas kullanıcı adlarını gerçekçi kimliklere taşır.</summary>
    private static async Task EnsureModernLoginAccountsAsync(
        GatewayDbContext db,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        var probe = ApplicationUser.CreatePersonel("probe@ornekkoy.bel.tr", "Probe", "probe-hash-placeholder");
        var changed = false;

        var personel = await db.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Personel, cancellationToken);
        if (personel is null)
        {
            personel = ApplicationUser.CreatePersonel(
                PersonelEmail,
                PersonelDisplayName,
                hasher.HashPassword(probe, PersonelPassword));
            await db.Users.AddAsync(personel, cancellationToken);
            changed = true;
        }
        else
        {
            if (!string.Equals(personel.Username, PersonelEmail, StringComparison.OrdinalIgnoreCase))
            {
                personel.SetLoginIdentifier(PersonelEmail);
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(personel.DisplayName)
                || !string.Equals(personel.DisplayName, PersonelDisplayName, StringComparison.Ordinal))
            {
                personel.SetDisplayName(PersonelDisplayName);
                changed = true;
            }
        }

        var demoCitizen = await db.Citizens.FirstOrDefaultAsync(
                               c => c.AdSoyad == SharedCitizenCatalog.DemoVatandasAdSoyad,
                               cancellationToken)
                           ?? await db.Citizens.FirstAsync(cancellationToken);

        var vatandas = await db.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Vatandas, cancellationToken);
        if (vatandas is null)
        {
            vatandas = ApplicationUser.CreateVatandas(
                demoCitizen.TcNo,
                demoCitizen.AdSoyad,
                hasher.HashPassword(probe, VatandasPassword),
                demoCitizen.Id);
            await db.Users.AddAsync(vatandas, cancellationToken);
            changed = true;
        }
        else
        {
            if (!string.Equals(vatandas.Username, demoCitizen.TcNo, StringComparison.Ordinal))
            {
                vatandas.SetLoginIdentifier(demoCitizen.TcNo);
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(vatandas.DisplayName)
                || !string.Equals(vatandas.DisplayName, demoCitizen.AdSoyad, StringComparison.Ordinal))
            {
                vatandas.SetDisplayName(demoCitizen.AdSoyad);
                changed = true;
            }
        }

        if (changed)
        {
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Giriş hesapları güncellendi → {Email} / TC {Tc}", PersonelEmail, demoCitizen.TcNo);
        }
    }
}
