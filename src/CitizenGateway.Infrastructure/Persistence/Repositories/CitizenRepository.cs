using CitizenGateway.Application.Abstractions;
using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitizenGateway.Infrastructure.Persistence.Repositories;

public sealed class CitizenRepository : ICitizenRepository
{
    private readonly GatewayDbContext _db;

    public CitizenRepository(GatewayDbContext db) => _db = db;

    public Task<Citizen?> GetByTcNoAsync(string tcNo, CancellationToken cancellationToken = default) =>
        _db.Citizens.AsNoTracking().FirstOrDefaultAsync(c => c.TcNo == tcNo, cancellationToken);

    public Task<Citizen?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Citizens.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task AddAsync(Citizen citizen, CancellationToken cancellationToken = default)
    {
        await _db.Citizens.AddAsync(citizen, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        _db.Citizens.CountAsync(cancellationToken);
}
