using CitizenGateway.Contracts.Citizens;

namespace CitizenGateway.Application.Features.Citizens;

public interface ICitizenDirectoryService
{
    Task<IReadOnlyList<CitizenListItemDto>> ListAsync(CancellationToken cancellationToken = default);
}
