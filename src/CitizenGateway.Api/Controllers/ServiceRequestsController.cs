using CitizenGateway.Api.Auth;
using CitizenGateway.Application.Features.Audit;
using CitizenGateway.Application.Features.Requests;
using CitizenGateway.Contracts.Requests;
using CitizenGateway.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenGateway.Api.Controllers;

/// <summary>
/// Personel talep kutusu — birim adına onay / red kararı.
/// Demo'da tek Personel rolü tüm birimler adına karar verir.
/// </summary>
[ApiController]
[Authorize(Roles = "Personel")]
[Route("api/service-requests")]
public sealed class ServiceRequestsController : ControllerBase
{
    private readonly IServiceRequestService _requests;
    private readonly IAuditLogger _auditLogger;

    public ServiceRequestsController(IServiceRequestService requests, IAuditLogger auditLogger)
    {
        _requests = requests;
        _auditLogger = auditLogger;
    }

    /// <summary>Talep kutusu. status boşsa tümü; Beklemede ile yalnızca karar bekleyenler.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServiceRequestDto>>> List(
        [FromQuery] RequestStatus? status,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 200);
        var items = await _requests.ListInboxAsync(status, take, cancellationToken);
        return Ok(items);
    }

    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceRequestDto>> Approve(Guid id, CancellationToken cancellationToken)
    {
        var updated = await _requests.ApproveAsync(id, cancellationToken);
        await AuditDecisionAsync(updated, "approve", cancellationToken);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceRequestDto>> Reject(Guid id, CancellationToken cancellationToken)
    {
        var updated = await _requests.RejectAsync(id, cancellationToken);
        await AuditDecisionAsync(updated, "reject", cancellationToken);
        return Ok(updated);
    }

    private async Task AuditDecisionAsync(ServiceRequestDto updated, string action, CancellationToken cancellationToken)
    {
        try
        {
            await _auditLogger.LogAccessAsync(
                User.GetUserId(),
                User.GetUserRole(),
                updated.TcNo,
                $"POST /api/service-requests/{updated.Id}/{action}",
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                cancellationToken);
        }
        catch
        {
            // Karar kaydı asıl iş; audit yazılamasa bile yanıt bozulmasın.
        }
    }
}
