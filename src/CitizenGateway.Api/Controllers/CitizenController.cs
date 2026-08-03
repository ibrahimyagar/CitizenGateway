using CitizenGateway.Api.Auth;
using CitizenGateway.Api.Filters;
using CitizenGateway.Application.DTOs;
using CitizenGateway.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenGateway.Api.Controllers;

/// <summary>
/// Vatandaş özet ve talep uçları.
/// Yetki: Personel herkes; Vatandas yalnızca kendi TC'si (CitizenAccessGuard).
/// AuditCitizenAccess: her summary/requests çağrısı AuditLogEntry üretir.
/// </summary>
[ApiController]
[Authorize]
[AuditCitizenAccess]
[Route("api/citizen")]
public sealed class CitizenController : ControllerBase
{
    private readonly CitizenSummaryService _summaryService;
    private readonly ServiceRequestService _requestService;
    private readonly CitizenAccessGuard _accessGuard;

    public CitizenController(
        CitizenSummaryService summaryService,
        ServiceRequestService requestService,
        CitizenAccessGuard accessGuard)
    {
        _summaryService = summaryService;
        _requestService = requestService;
        _accessGuard = accessGuard;
    }

    [HttpGet("{tcNo}/summary")]
    [ProducesResponseType(typeof(CitizenSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CitizenSummaryDto>> GetSummary(string tcNo, CancellationToken cancellationToken)
    {
        EnsureAccess(tcNo);
        var summary = await _summaryService.GetSummaryAsync(tcNo, cancellationToken);
        return Ok(summary);
    }

    [HttpPost("{tcNo}/requests")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ServiceRequestDto>> CreateRequest(
        string tcNo,
        [FromBody] CreateServiceRequestDto dto,
        CancellationToken cancellationToken)
    {
        EnsureAccess(tcNo);
        var created = await _requestService.CreateAsync(tcNo, dto, cancellationToken);
        return CreatedAtAction(nameof(GetRequests), new { tcNo }, created);
    }

    [HttpGet("{tcNo}/requests")]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServiceRequestDto>>> GetRequests(
        string tcNo,
        CancellationToken cancellationToken)
    {
        EnsureAccess(tcNo);
        var list = await _requestService.GetByTcNoAsync(tcNo, cancellationToken);
        return Ok(list);
    }

    private void EnsureAccess(string tcNo) =>
        _accessGuard.EnsureCanAccess(User.GetUserRole(), User.GetLinkedTcNo(), tcNo);
}
