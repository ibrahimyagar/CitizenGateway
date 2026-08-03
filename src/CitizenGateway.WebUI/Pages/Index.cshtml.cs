using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CitizenGateway.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

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

    public bool IsPersonel => User.IsInRole("Personel");
    public string? LinkedTcNo => User.FindFirstValue(WebUiClaimTypes.LinkedTcNo);
    public string Username => User.Identity?.Name ?? "";

    public HealthStatus Health { get; private set; } = new();
    public List<SelectListItem> CitizenOptions { get; private set; } = [];
    public CitizenSummaryViewModel? Summary { get; private set; }
    public IReadOnlyList<ServiceRequestItem> Requests { get; private set; } = [];
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }

    [BindProperty]
    [Display(Name = "T.C. Kimlik No")]
    public string TcNo { get; set; } = "";

    [BindProperty]
    [Display(Name = "Talep türü")]
    public string NewRequestType { get; set; } = "KursKaydi";

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadShellAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(TcNo))
            TcNo = LinkedTcNo ?? CitizenOptions.FirstOrDefault()?.Value ?? "71151275166";
    }

    public async Task<IActionResult> OnPostQueryAsync(CancellationToken cancellationToken)
    {
        await LoadShellAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(TcNo))
        {
            ErrorMessage = "TC seçin veya girin.";
            return Page();
        }

        if (!IsPersonel && !string.Equals(TcNo, LinkedTcNo, StringComparison.Ordinal))
        {
            ErrorMessage = "Vatandaş yalnızca kendi TC'sini sorgulayabilir.";
            TcNo = LinkedTcNo ?? TcNo;
            return Page();
        }

        try
        {
            Summary = await _gateway.GetSummaryAsync(TcNo.Trim(), cancellationToken);
            Requests = await _gateway.GetRequestsAsync(TcNo.Trim(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sorgula başarısız");
            ErrorMessage = ex.Message;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCreateRequestAsync(CancellationToken cancellationToken)
    {
        await LoadShellAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(TcNo))
        {
            ErrorMessage = "Önce bir TC seçin.";
            return Page();
        }

        try
        {
            await _gateway.CreateRequestAsync(TcNo.Trim(), NewRequestType, cancellationToken);
            SuccessMessage = "Talep oluşturuldu.";
            Summary = await _gateway.GetSummaryAsync(TcNo.Trim(), cancellationToken);
            Requests = await _gateway.GetRequestsAsync(TcNo.Trim(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Talep oluşturma başarısız");
            ErrorMessage = ex.Message;
            try
            {
                Summary = await _gateway.GetSummaryAsync(TcNo.Trim(), cancellationToken);
                Requests = await _gateway.GetRequestsAsync(TcNo.Trim(), cancellationToken);
            }
            catch { /* ignore */ }
        }

        return Page();
    }

    private async Task LoadShellAsync(CancellationToken cancellationToken)
    {
        Health = await _gateway.GetHealthAsync(cancellationToken);

        if (!IsPersonel)
            return;

        try
        {
            var citizens = await _gateway.ListCitizensAsync(cancellationToken);
            CitizenOptions = citizens
                .Select(c => new SelectListItem($"{c.AdSoyad} — {c.TcNo}", c.TcNo))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Vatandaş listesi alınamadı");
            CitizenOptions = [];
        }
    }
}
