using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Domain.Entities;

/// <summary>
/// Gateway kimlik kaydı (JWT login kaynağı).
/// Vatandas rolünde LinkedCitizenId ile kendi sentetik vatandaş kaydına bağlanır.
/// PasswordHash burada tutulur; düz metin asla Domain'e girmez.
/// </summary>
public class ApplicationUser
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }

    /// <summary>Vatandas rolü için zorunlu; Personel için null.</summary>
    public Guid? LinkedCitizenId { get; private set; }

    public Citizen? LinkedCitizen { get; private set; }

    private ApplicationUser()
    {
    }

    public static ApplicationUser CreatePersonel(string username, string passwordHash)
    {
        ValidateCredentials(username, passwordHash);

        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = username.Trim(),
            PasswordHash = passwordHash,
            Role = UserRole.Personel,
            LinkedCitizenId = null
        };
    }

    public static ApplicationUser CreateVatandas(string username, string passwordHash, Guid linkedCitizenId)
    {
        ValidateCredentials(username, passwordHash);

        if (linkedCitizenId == Guid.Empty)
            throw new ArgumentException("Vatandaş kullanıcısı bir Citizen kaydına bağlanmalıdır.", nameof(linkedCitizenId));

        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = username.Trim(),
            PasswordHash = passwordHash,
            Role = UserRole.Vatandas,
            LinkedCitizenId = linkedCitizenId
        };
    }

    private static void ValidateCredentials(string username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username boş olamaz.", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash boş olamaz.", nameof(passwordHash));
    }
}
