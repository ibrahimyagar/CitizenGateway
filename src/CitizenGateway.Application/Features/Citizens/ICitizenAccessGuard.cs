using CitizenGateway.Domain.Enums;

namespace CitizenGateway.Application.Features.Citizens;

public interface ICitizenAccessGuard
{
    void EnsureCanAccess(UserRole role, string? callerTcNo, string requestedTcNo);
}
