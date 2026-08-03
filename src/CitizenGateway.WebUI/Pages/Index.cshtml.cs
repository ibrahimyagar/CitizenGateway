using System.ComponentModel.DataAnnotations;
using CitizenGateway.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitizenGateway.WebUI.Pages;

public sealed class IndexModel : PageModel
{
    private readonly GatewayApiClient _gateway;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(GatewayApiClient gateway, ILogger<IndexModel> logger)
    {
        _gateway = gateway;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "T.C. kimlik no gerekli.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "11 haneli sentetik TC girin.")]
    [Display(Name = "T.C. Kimlik No")]
    public string TcNo { get; set; } = "71151275166";

    public CitizenSummaryViewModel? Summary { get; private set; }
    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            Summary = await _gateway.GetSummaryAsync(TcNo.Trim(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Summary sorgusu başarısız. Tc={Tc}", TcNo);
            ErrorMessage = ex.Message;
        }

        return Page();
    }
}
