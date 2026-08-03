using CitizenGateway.Application.Abstractions;
using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitizenGateway.Infrastructure.Persistence.Repositories;

public sealed class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly GatewayDbContext _db;

    public ServiceRequestRepository(GatewayDbContext db) => _db = db;

    public async Task AddAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        await _db.ServiceRequests.AddAsync(request, cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceRequest>> GetByCitizenIdAsync(
        Guid citizenId,
        CancellationToken cancellationToken = default)
    {
        return await _db.ServiceRequests
            .AsNoTracking()
            .Where(r => r.CitizenId == citizenId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
