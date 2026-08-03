using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Domain.Entities;

/// <summary>
/// Kim, ne zaman, hangi vatandaşın verisine baktı?
/// KVKK/denetim demosu: her /summary ve /requests erişiminde yazılır.
/// Immutable tasarım — audit kaydı sonradan değiştirilmemeli.
/// </summary>
public class AuditLogEntry
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public UserRole UserRole { get; private set; }

    /// <summary>Erişilen sentetik TC — gerçek kişi verisi değildir.</summary>
    public string AccessedCitizenTcNo { get; private set; } = null!;

    public string AccessedEndpoint { get; private set; } = null!;
    public DateTimeOffset Timestamp { get; private set; }
    public string IpAddress { get; private set; } = null!;

    private AuditLogEntry()
    {
    }

    public static AuditLogEntry Create(
        Guid userId,
        UserRole userRole,
        string accessedCitizenTcNo,
        string accessedEndpoint,
        string ipAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessedCitizenTcNo);
        ArgumentException.ThrowIfNullOrWhiteSpace(accessedEndpoint);

        return new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserRole = userRole,
            AccessedCitizenTcNo = accessedCitizenTcNo.Trim(),
            AccessedEndpoint = accessedEndpoint.Trim(),
            Timestamp = DateTimeOffset.UtcNow,
            IpAddress = string.IsNullOrWhiteSpace(ipAddress) ? "unknown" : ipAddress.Trim()
        };
    }
}
