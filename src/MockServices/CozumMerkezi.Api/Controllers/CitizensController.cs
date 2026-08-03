using CozumMerkezi.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace CozumMerkezi.Api.Controllers;

/// <summary>
/// Mock Çözüm Merkezi API — şikayet/talep simülasyonu.
/// </summary>
[ApiController]
[Route("api")]
public sealed class CitizensController : ControllerBase
{
    private readonly CozumMerkeziSeedStore _store;

    public CitizensController(CozumMerkeziSeedStore store) => _store = store;

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
