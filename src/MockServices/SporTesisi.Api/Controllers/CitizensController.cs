using Microsoft.AspNetCore.Mvc;
using SporTesisi.Api.Data;

namespace SporTesisi.Api.Controllers;

/// <summary>
/// Mock Spor Tesisi API — Gateway'in paralel çağıracağı uç nokta.
/// Auth yok: dış departman sistemi simülasyonu; güvenlik Gateway'de.
/// </summary>
[ApiController]
[Route("api")]
public sealed class CitizensController : ControllerBase
{
    private readonly SporTesisiSeedStore _store;

    public CitizensController(SporTesisiSeedStore store) => _store = store;

    /// <summary>Sentetik TC ile üyelik/kontör bilgisi döner.</summary>
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
