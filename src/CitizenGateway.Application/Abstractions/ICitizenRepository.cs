using CitizenGateway.Domain.Entities;

namespace CitizenGateway.Application.Abstractions;

/// <summary>
/// Vatandaş kalıcılık kontratı — Application DB bilmez, Infrastructure uygular (DIP).
/// </summary>
public interface ICitizenRepository
{
    Task<Citizen?> GetByTcNoAsync(string tcNo, CancellationToken cancellationToken = default);
    Task<Citizen?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Citizen>> ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Citizen citizen, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
