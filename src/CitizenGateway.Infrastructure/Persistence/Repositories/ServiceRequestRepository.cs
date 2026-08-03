using CitizenGateway.Application.Abstractions;
using CitizenGateway.Domain.Entities;
using CitizenGateway.Domain.Enums;
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
            .Include(r => r.Citizen)
            .Where(r => r.CitizenId == citizenId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.ServiceRequests
            .Include(r => r.Citizen)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ServiceRequest>> ListAsync(
        RequestStatus? status,
        int take,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 200);

        var query = _db.ServiceRequests
            .AsNoTracking()
            .Include(r => r.Citizen)
            .AsQueryable();

        if (status is not null)
            query = query.Where(r => r.Status == status);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
