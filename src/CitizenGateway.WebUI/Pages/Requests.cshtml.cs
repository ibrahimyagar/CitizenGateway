using CitizenGateway.Contracts.Requests;
using CitizenGateway.Domain.Enums;
using CitizenGateway.WebUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitizenGateway.WebUI.Pages;

/// <summary>
/// Personel talep kutusu — birim adına onay / red.
/// </summary>
[Authorize(Roles = "Personel")]
public sealed class RequestsModel : PageModel
{
    private readonly GatewayApiClient _gateway;
    private readonly ILogger<RequestsModel> _logger;

    public RequestsModel(GatewayApiClient gateway, ILogger<RequestsModel> logger)
    {
        _gateway = gateway;
        _logger = logger;
    }

    public IReadOnlyList<ServiceRequestDto> Items { get; private set; } = [];
    public string Filter { get; private set; } = "pending";
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }

    public async Task OnGetAsync(string? filter, CancellationToken cancellationToken)
    {
        await LoadAsync(filter, cancellationToken);
    }

    public async Task<IActionResult> OnPostApproveAsync(Guid id, string? filter, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _gateway.ApproveRequestAsync(id, cancellationToken);
            SuccessMessage = $"{updated.AdSoyad ?? updated.TcNo} için talep onaylandı → {DemoLabels.ForTargetService(updated.TargetService)}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Onay başarısız");
            ErrorMessage = ex.Message;
        }

        await LoadAsync(filter, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(Guid id, string? filter, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _gateway.RejectRequestAsync(id, cancellationToken);
            SuccessMessage = $"{updated.AdSoyad ?? updated.TcNo} için talep reddedildi.";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Red başarısız");
            ErrorMessage = ex.Message;
        }

        await LoadAsync(filter, cancellationToken);
        return Page();
    }

    private async Task LoadAsync(string? filter, CancellationToken cancellationToken)
    {
        Filter = string.IsNullOrWhiteSpace(filter) ? "pending" : filter.Trim().ToLowerInvariant();

        RequestStatus? status = Filter switch
        {
            "pending" => RequestStatus.Beklemede,
            "approved" => RequestStatus.Onaylandi,
            "rejected" => RequestStatus.Reddedildi,
            _ => null
        };

        if (Filter is not ("pending" or "approved" or "rejected" or "all"))
            Filter = "pending";

        try
        {
            Items = await _gateway.ListServiceRequestsAsync(status, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Talep listesi alınamadı");
            ErrorMessage ??= ex.Message;
            Items = [];
        }
    }
}
