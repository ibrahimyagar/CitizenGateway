using CitizenGateway.Domain.Entities;

namespace CitizenGateway.Application.Abstractions;

public interface IServiceRequestRepository
{
    Task AddAsync(ServiceRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceRequest>> GetByCitizenIdAsync(Guid citizenId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
