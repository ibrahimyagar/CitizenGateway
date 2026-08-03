using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Domain.Entities;

/// <summary>
/// Gateway kimlik kaydı.
/// Personel: LoginIdentifier = kurumsal e-posta.
/// Vatandaş: LoginIdentifier = bağlı sentetik T.C. kimlik no.
/// </summary>
public class ApplicationUser
{
    public Guid Id { get; private set; }

    /// <summary>Giriş kimliği (e-posta veya TC). Kolon adı Username — geriye dönük şema uyumu.</summary>
    public string Username { get; private set; } = null!;

    public string DisplayName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }

    public Guid? LinkedCitizenId { get; private set; }
    public Citizen? LinkedCitizen { get; private set; }

    private ApplicationUser()
    {
    }

    public static ApplicationUser CreatePersonel(string email, string displayName, string passwordHash)
    {
        ValidateCredentials(email, passwordHash);
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("DisplayName boş olamaz.", nameof(displayName));

        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = email.Trim().ToLowerInvariant(),
            DisplayName = displayName.Trim(),
            PasswordHash = passwordHash,
            Role = UserRole.Personel,
            LinkedCitizenId = null
        };
    }

    public static ApplicationUser CreateVatandas(string tcNo, string displayName, string passwordHash, Guid linkedCitizenId)
    {
        ValidateCredentials(tcNo, passwordHash);
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("DisplayName boş olamaz.", nameof(displayName));
        if (linkedCitizenId == Guid.Empty)
            throw new ArgumentException("Vatandaş kullanıcısı bir Citizen kaydına bağlanmalıdır.", nameof(linkedCitizenId));

        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = tcNo.Trim(),
            DisplayName = displayName.Trim(),
            PasswordHash = passwordHash,
            Role = UserRole.Vatandas,
            LinkedCitizenId = linkedCitizenId
        };
    }

    public void SetLoginIdentifier(string identifier) =>
        Username = identifier.Trim();

    public void SetDisplayName(string displayName) =>
        DisplayName = displayName.Trim();

    private static void ValidateCredentials(string loginIdentifier, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(loginIdentifier))
            throw new ArgumentException("Giriş kimliği boş olamaz.", nameof(loginIdentifier));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash boş olamaz.", nameof(passwordHash));
    }
}
