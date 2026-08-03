using CitizenGateway.Contracts.Citizens;

namespace CitizenGateway.Application.Features.Citizens;

public interface ICitizenSummaryService
{
    Task<CitizenSummaryDto> GetSummaryAsync(string tcNo, CancellationToken cancellationToken = default);
}
