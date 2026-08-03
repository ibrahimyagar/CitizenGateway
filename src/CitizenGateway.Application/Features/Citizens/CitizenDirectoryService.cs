using CitizenGateway.Application.Abstractions;
using CitizenGateway.Contracts.Citizens;

namespace CitizenGateway.Application.Features.Citizens;

/// <summary>
/// Seed edilmiş vatandaşları listeler — UI'da TC ezberlemeyi kaldırır.
/// </summary>
public sealed class CitizenDirectoryService : ICitizenDirectoryService
{
    private readonly ICitizenRepository _citizens;

    public CitizenDirectoryService(ICitizenRepository citizens) => _citizens = citizens;

    public async Task<IReadOnlyList<CitizenListItemDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _citizens.ListAsync(cancellationToken);
        return items
            .Select(c => new CitizenListItemDto(c.Id, c.TcNo, c.AdSoyad, c.DogumTarihi, c.Telefon))
            .ToList();
    }
}
