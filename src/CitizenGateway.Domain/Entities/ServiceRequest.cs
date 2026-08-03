using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Domain.Entities;

/// <summary>
/// Departmanlar arası talep kaydı (kurs, şikayet, randevu).
/// Gateway hem DB'ye yazar hem ilgili mock servise iletir — bu entity kalıcı izdir.
/// </summary>
public class ServiceRequest
{
    public Guid Id { get; private set; }
    public Guid CitizenId { get; private set; }
    public RequestType RequestType { get; private set; }
    public RequestStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public TargetService TargetService { get; private set; }

    // Navigasyon — sorgularda Include kolaylığı; zorunlu değil.
    public Citizen? Citizen { get; private set; }

    private ServiceRequest()
    {
    }

    public static ServiceRequest Create(
        Guid citizenId,
        RequestType requestType,
        TargetService targetService,
        RequestStatus status = RequestStatus.Beklemede)
    {
        if (citizenId == Guid.Empty)
            throw new ArgumentException("CitizenId boş olamaz.", nameof(citizenId));

        return new ServiceRequest
        {
            Id = Guid.NewGuid(),
            CitizenId = citizenId,
            RequestType = requestType,
            TargetService = targetService,
            Status = status,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Approve() => Status = RequestStatus.Onaylandi;
    public void Reject() => Status = RequestStatus.Reddedildi;
}
