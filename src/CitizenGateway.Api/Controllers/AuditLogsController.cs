using CitizenGateway.Application.Features.Audit;
using CitizenGateway.Contracts.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenGateway.Api.Controllers;

[ApiController]
[Authorize(Roles = "Personel")]
[Route("api/audit-logs")]
public sealed class AuditLogsController : ControllerBase
{
    private readonly IAuditQueryService _auditQuery;

    public AuditLogsController(IAuditQueryService auditQuery) => _auditQuery = auditQuery;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetLatest(
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 500);
        var logs = await _auditQuery.GetLatestAsync(take, cancellationToken);
        return Ok(logs);
    }
}
