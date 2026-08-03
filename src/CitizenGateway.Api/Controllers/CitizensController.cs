using CitizenGateway.Api.Auth;
using CitizenGateway.Api.Filters;
using CitizenGateway.Application.Features.Citizens;
using CitizenGateway.Application.Features.Requests;
using CitizenGateway.Contracts.Citizens;
using CitizenGateway.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenGateway.Api.Controllers;

/// <summary>
/// Vatandaş dizini, özet ve talep uçları — tek kaynak: /api/citizens.
/// </summary>
[ApiController]
[Authorize]
[Route("api/citizens")]
public sealed class CitizensController : ControllerBase
{
    private readonly ICitizenDirectoryService _directory;
    private readonly ICitizenSummaryService _summaryService;
    private readonly IServiceRequestService _requestService;
    private readonly ICitizenAccessGuard _accessGuard;

    public CitizensController(
        ICitizenDirectoryService directory,
        ICitizenSummaryService summaryService,
        IServiceRequestService requestService,
        ICitizenAccessGuard accessGuard)
    {
        _directory = directory;
        _summaryService = summaryService;
        _requestService = requestService;
        _accessGuard = accessGuard;
    }

    [HttpGet]
    [Authorize(Roles = "Personel")]
    [ProducesResponseType(typeof(IReadOnlyList<CitizenListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CitizenListItemDto>>> List(CancellationToken cancellationToken)
    {
        var items = await _directory.ListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{tcNo}/summary")]
    [AuditCitizenAccess]
    [ProducesResponseType(typeof(CitizenSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CitizenSummaryDto>> GetSummary(string tcNo, CancellationToken cancellationToken)
    {
        EnsureAccess(tcNo);
        var summary = await _summaryService.GetSummaryAsync(tcNo, cancellationToken);
        return Ok(summary);
    }

    [HttpPost("{tcNo}/requests")]
    [AuditCitizenAccess]
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
    [AuditCitizenAccess]
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
