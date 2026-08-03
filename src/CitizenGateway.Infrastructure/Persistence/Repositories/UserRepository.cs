using CitizenGateway.Application.Abstractions;
using CitizenGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitizenGateway.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly GatewayDbContext _db;

    public UserRepository(GatewayDbContext db) => _db = db;

    public Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        _db.Users
            .AsNoTracking()
            .Include(u => u.LinkedCitizen)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Users
            .AsNoTracking()
            .Include(u => u.LinkedCitizen)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
}
