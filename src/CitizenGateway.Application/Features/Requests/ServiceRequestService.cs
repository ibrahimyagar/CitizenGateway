using CitizenGateway.Application.Abstractions;
using CitizenGateway.Contracts.Requests;
using CitizenGateway.Domain.Entities;
using CitizenGateway.Domain.Enums;
using CitizenGateway.Domain.Exceptions;
using CitizenGateway.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CitizenGateway.Application.Features.Requests;

/// <summary>
/// Talep oluşturma, listeleme ve personel onay/red use-case'i.
/// </summary>
public sealed class ServiceRequestService : IServiceRequestService
{
    private readonly ICitizenRepository _citizens;
    private readonly IServiceRequestRepository _requests;
    private readonly IExternalServiceClient _external;
    private readonly ILogger<ServiceRequestService> _logger;

    public ServiceRequestService(
        ICitizenRepository citizens,
        IServiceRequestRepository requests,
        IExternalServiceClient external,
        ILogger<ServiceRequestService> logger)
    {
        _citizens = citizens;
        _requests = requests;
        _external = external;
        _logger = logger;
    }

    public async Task<ServiceRequestDto> CreateAsync(
        string tcNo,
        CreateServiceRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var validTc = TcNo.Create(tcNo);

        var citizen = await _citizens.GetByTcNoAsync(validTc.Value, cancellationToken)
            ?? throw new CitizenNotFoundException(validTc.Value);

        if (!Enum.IsDefined(dto.RequestType))
            throw new DomainValidationException($"Geçersiz talep türü: {dto.RequestType}");

        var target = MapTarget(dto.RequestType);

        await TryNotifyTargetAsync(target, validTc.Value, cancellationToken);

        var entity = ServiceRequest.Create(citizen.Id, dto.RequestType, target);
        await _requests.AddAsync(entity, cancellationToken);
        await _requests.SaveChangesAsync(cancellationToken);

        return ToDto(entity, citizen.TcNo, citizen.AdSoyad);
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> GetByTcNoAsync(
        string tcNo,
        CancellationToken cancellationToken = default)
    {
        var validTc = TcNo.Create(tcNo);

        var citizen = await _citizens.GetByTcNoAsync(validTc.Value, cancellationToken)
            ?? throw new CitizenNotFoundException(validTc.Value);

        var list = await _requests.GetByCitizenIdAsync(citizen.Id, cancellationToken);
        return list.Select(r => ToDto(r, citizen.TcNo, citizen.AdSoyad)).ToList();
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> ListInboxAsync(
        RequestStatus? status,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var list = await _requests.ListAsync(status, take, cancellationToken);
        return list
            .Select(r => ToDto(r, r.Citizen?.TcNo ?? "", r.Citizen?.AdSoyad))
            .ToList();
    }

    public async Task<ServiceRequestDto> ApproveAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var entity = await GetPendingOrThrowAsync(requestId, cancellationToken);
        entity.Approve();
        await _requests.SaveChangesAsync(cancellationToken);
        return ToDto(entity, entity.Citizen?.TcNo ?? "", entity.Citizen?.AdSoyad);
    }

    public async Task<ServiceRequestDto> RejectAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var entity = await GetPendingOrThrowAsync(requestId, cancellationToken);
        entity.Reject();
        await _requests.SaveChangesAsync(cancellationToken);
        return ToDto(entity, entity.Citizen?.TcNo ?? "", entity.Citizen?.AdSoyad);
    }

    private async Task<ServiceRequest> GetPendingOrThrowAsync(Guid requestId, CancellationToken cancellationToken)
    {
        if (requestId == Guid.Empty)
            throw new DomainValidationException("Talep kimliği geçersiz.");

        return await _requests.GetByIdAsync(requestId, cancellationToken)
            ?? throw new DomainValidationException("Talep bulunamadı.");
    }

    private static TargetService MapTarget(RequestType type) => type switch
    {
        RequestType.KursKaydi => TargetService.SporTesisi,
        RequestType.RandevuTalebi => TargetService.Kutuphane,
        RequestType.SikayetAcma => TargetService.CozumMerkezi,
        _ => throw new DomainValidationException($"Desteklenmeyen talep türü: {type}")
    };

    private async Task TryNotifyTargetAsync(TargetService target, string tcNo, CancellationToken cancellationToken)
    {
        try
        {
            switch (target)
            {
                case TargetService.SporTesisi:
                    await _external.GetSporTesisiAsync(tcNo, cancellationToken);
                    break;
                case TargetService.Kutuphane:
                    await _external.GetKutuphaneAsync(tcNo, cancellationToken);
                    break;
                case TargetService.CozumMerkezi:
                    await _external.GetCozumMerkeziAsync(tcNo, cancellationToken);
                    break;
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Talep iletimi başarısız (best-effort). Target={Target}, Tc={Tc}", target, tcNo);
        }
    }

    private static ServiceRequestDto ToDto(ServiceRequest entity, string tcNo, string? adSoyad) =>
        new(entity.Id, entity.CitizenId, tcNo, entity.RequestType, entity.Status, entity.TargetService, entity.CreatedAt, adSoyad);
}
