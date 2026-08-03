using CitizenGateway.Domain.Entities;

namespace CitizenGateway.Application.Abstractions;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
