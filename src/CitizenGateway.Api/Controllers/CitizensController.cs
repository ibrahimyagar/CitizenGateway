using CitizenGateway.Application.DTOs;
using CitizenGateway.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenGateway.Api.Controllers;

/// <summary>
/// Vatandaş dizini — Personel demo'da TC ezberlemeden seçim yapabilsin.
/// </summary>
[ApiController]
[Authorize(Roles = "Personel")]
[Route("api/citizens")]
public sealed class CitizensController : ControllerBase
{
    private readonly CitizenDirectoryService _directory;

    public CitizensController(CitizenDirectoryService directory) => _directory = directory;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CitizenListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CitizenListItemDto>>> List(CancellationToken cancellationToken)
    {
        var items = await _directory.ListAsync(cancellationToken);
        return Ok(items);
    }
}
