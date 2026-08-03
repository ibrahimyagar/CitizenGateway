using CitizenGateway.Application.Abstractions;
using CitizenGateway.Contracts.Citizens;
using CitizenGateway.Domain.Exceptions;
using CitizenGateway.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace CitizenGateway.Application.Features.Citizens;

/// <summary>
/// Gateway'in ana use case'i: 3 mock servise paralel istek, konsolide özet.
/// Bir servis çökerse tüm isteği patlatmaz — PartialFailure + FailedServices döner.
/// </summary>
public sealed class CitizenSummaryService : ICitizenSummaryService
{
    private readonly ICitizenRepository _citizens;
    private readonly IExternalServiceClient _external;
    private readonly ILogger<CitizenSummaryService> _logger;

    public CitizenSummaryService(
        ICitizenRepository citizens,
        IExternalServiceClient external,
        ILogger<CitizenSummaryService> logger)
    {
        _citizens = citizens;
        _external = external;
        _logger = logger;
    }

    public async Task<CitizenSummaryDto> GetSummaryAsync(string tcNo, CancellationToken cancellationToken = default)
    {
        var validTc = TcNo.Create(tcNo);

        var citizen = await _citizens.GetByTcNoAsync(validTc.Value, cancellationToken)
            ?? throw new CitizenNotFoundException(validTc.Value);

        var sporTask = SafeCallAsync("SporTesisi", () => _external.GetSporTesisiAsync(validTc.Value, cancellationToken));
        var kutuphaneTask = SafeCallAsync("Kutuphane", () => _external.GetKutuphaneAsync(validTc.Value, cancellationToken));
        var cozumTask = SafeCallAsync("CozumMerkezi", () => _external.GetCozumMerkeziAsync(validTc.Value, cancellationToken));

        await Task.WhenAll(sporTask, kutuphaneTask, cozumTask);

        var spor = await sporTask;
        var kutuphane = await kutuphaneTask;
        var cozum = await cozumTask;

        var failed = new List<string>(3);
        if (!spor.Success) failed.Add("SporTesisi");
        if (!kutuphane.Success) failed.Add("Kutuphane");
        if (!cozum.Success) failed.Add("CozumMerkezi");

        return new CitizenSummaryDto(
            TcNo: citizen.TcNo,
            AdSoyad: citizen.AdSoyad,
            SporTesisi: spor.Data,
            Kutuphane: kutuphane.Data,
            CozumMerkezi: cozum.Data,
            PartialFailure: failed.Count > 0,
            FailedServices: failed);
    }

    private async Task<ServiceCallResult<T>> SafeCallAsync<T>(string serviceName, Func<Task<T?>> call)
    {
        try
        {
            var data = await call();
            return ServiceCallResult<T>.Ok(data);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Dış servis başarısız: {Service}", serviceName);
            return ServiceCallResult<T>.Fail();
        }
    }

    private sealed record ServiceCallResult<T>(bool Success, T? Data)
    {
        public static ServiceCallResult<T> Ok(T? data) => new(true, data);
        public static ServiceCallResult<T> Fail() => new(false, default);
    }
}
