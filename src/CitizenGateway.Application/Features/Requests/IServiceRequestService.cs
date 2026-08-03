using CitizenGateway.Contracts.Requests;
using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Application.Features.Requests;

public interface IServiceRequestService
{
    Task<ServiceRequestDto> CreateAsync(
        string tcNo,
        CreateServiceRequestDto dto,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServiceRequestDto>> GetByTcNoAsync(
        string tcNo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServiceRequestDto>> ListInboxAsync(
        RequestStatus? status,
        int take = 50,
        CancellationToken cancellationToken = default);

    Task<ServiceRequestDto> ApproveAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task<ServiceRequestDto> RejectAsync(Guid requestId, CancellationToken cancellationToken = default);
}
