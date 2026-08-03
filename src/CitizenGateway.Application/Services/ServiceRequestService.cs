using CitizenGateway.Application.Abstractions;
using CitizenGateway.Application.DTOs;
using CitizenGateway.Domain.Entities;
using CitizenGateway.Domain.Enums;
using CitizenGateway.Domain.Exceptions;
using CitizenGateway.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CitizenGateway.Application.Services;

/// <summary>
/// Talep oluşturma/listeleme use case'i.
/// DB'ye yazar; ilgili mock servise best-effort "iletim" (GET ile varlık doğrulama).
/// </summary>
public sealed class ServiceRequestService
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

        // Best-effort iletim: mock'larda POST yok; GET ile departman kaydını doğrularız.
        await TryNotifyTargetAsync(target, validTc.Value, cancellationToken);

        var entity = ServiceRequest.Create(citizen.Id, dto.RequestType, target);
        await _requests.AddAsync(entity, cancellationToken);
        await _requests.SaveChangesAsync(cancellationToken);

        return ToDto(entity, citizen.TcNo);
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> GetByTcNoAsync(
        string tcNo,
        CancellationToken cancellationToken = default)
    {
        var validTc = TcNo.Create(tcNo);

        var citizen = await _citizens.GetByTcNoAsync(validTc.Value, cancellationToken)
            ?? throw new CitizenNotFoundException(validTc.Value);

        var list = await _requests.GetByCitizenIdAsync(citizen.Id, cancellationToken);
        return list.Select(r => ToDto(r, citizen.TcNo)).ToList();
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
            // Talep Gateway DB'sinde kalır; dış servis down olsa bile vatandaş talebi kaybolmaz.
            _logger.LogWarning(ex, "Talep iletimi başarısız (best-effort). Target={Target}, Tc={Tc}", target, tcNo);
        }
    }

    private static ServiceRequestDto ToDto(ServiceRequest entity, string tcNo) =>
        new(entity.Id, entity.CitizenId, tcNo, entity.RequestType, entity.Status, entity.TargetService, entity.CreatedAt);
}
