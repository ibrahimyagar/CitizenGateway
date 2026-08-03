using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Application.DTOs;

public sealed record CreateServiceRequestDto(
    RequestType RequestType);

public sealed record ServiceRequestDto(
    Guid Id,
    Guid CitizenId,
    string TcNo,
    RequestType RequestType,
    RequestStatus Status,
    TargetService TargetService,
    DateTimeOffset CreatedAt);
