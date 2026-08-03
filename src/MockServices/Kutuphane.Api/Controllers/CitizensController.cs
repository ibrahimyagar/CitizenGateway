using Kutuphane.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane.Api.Controllers;

/// <summary>
/// Mock Kütüphane API — randevu geçmişi simülasyonu.
/// </summary>
[ApiController]
[Route("api")]
public sealed class CitizensController : ControllerBase
{
    private readonly KutuphaneSeedStore _store;

    public CitizensController(KutuphaneSeedStore store) => _store = store;

    [HttpGet("{tcNo}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByTcNo(string tcNo)
    {
        var record = _store.Find(tcNo);
        return record is null
            ? NotFound(new { message = "Sentetik kayıtlarda bu TC bulunamadı.", tcNo })
            : Ok(record);
    }
}
