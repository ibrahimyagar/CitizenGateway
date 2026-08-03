using CitizenGateway.Domain.Entities;
using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Application.Abstractions;

public interface IServiceRequestRepository
{
    Task AddAsync(ServiceRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServiceRequest>> GetByCitizenIdAsync(Guid citizenId, CancellationToken cancellationToken = default);

    /// <summary>Karar için izlenen (tracked) kayıt + vatandaş bilgisi.</summary>
    Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Personel talep kutusu — isteğe bağlı durum filtresi.</summary>
    Task<IReadOnlyList<ServiceRequest>> ListAsync(
        RequestStatus? status,
        int take,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
