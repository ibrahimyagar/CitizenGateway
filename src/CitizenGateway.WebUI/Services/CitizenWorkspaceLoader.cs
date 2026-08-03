using System.Security.Claims;
using CitizenGateway.Contracts.Citizens;
using CitizenGateway.Contracts.Health;
using CitizenGateway.Contracts.Requests;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CitizenGateway.WebUI.Services;

/// <summary>
/// Index sayfasının ortak yükleme işleri — PageModel şişkin olmasın.
/// </summary>
public sealed class CitizenWorkspaceLoader
{
    private readonly GatewayApiClient _gateway;
    private readonly ILogger<CitizenWorkspaceLoader> _logger;

    public CitizenWorkspaceLoader(GatewayApiClient gateway, ILogger<CitizenWorkspaceLoader> logger)
    {
        _gateway = gateway;
        _logger = logger;
    }

    public async Task<WorkspaceShell> LoadShellAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        var health = await _gateway.GetHealthAsync(cancellationToken);
        var isPersonel = user.IsInRole("Personel");
        var linkedTc = user.FindFirstValue(WebUiClaimTypes.LinkedTcNo);
        var options = new List<SelectListItem>();

        if (isPersonel)
        {
            try
            {
                var citizens = await _gateway.ListCitizensAsync(cancellationToken);
                options = citizens
                    .Select(c => new SelectListItem($"{c.AdSoyad} — {c.TcNo}", c.TcNo))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Vatandaş listesi alınamadı");
            }
        }

        return new WorkspaceShell(health, isPersonel, linkedTc, options);
    }

    public async Task<WorkspaceData> LoadCitizenAsync(string tcNo, CancellationToken cancellationToken)
    {
        var summary = await _gateway.GetSummaryAsync(tcNo, cancellationToken);
        var requests = await _gateway.GetRequestsAsync(tcNo, cancellationToken);
        return new WorkspaceData(summary, requests);
    }

    public string ResolveDefaultTc(WorkspaceShell shell, string? currentTc)
    {
        if (!string.IsNullOrWhiteSpace(currentTc))
            return currentTc.Trim();

        if (!string.IsNullOrWhiteSpace(shell.LinkedTcNo))
            return shell.LinkedTcNo!;

        return shell.CitizenOptions.FirstOrDefault()?.Value ?? "";
    }
}

public sealed record WorkspaceShell(
    GatewayHealthDto Health,
    bool IsPersonel,
    string? LinkedTcNo,
    List<SelectListItem> CitizenOptions);

public sealed record WorkspaceData(
    CitizenSummaryDto Summary,
    IReadOnlyList<ServiceRequestDto> Requests);
