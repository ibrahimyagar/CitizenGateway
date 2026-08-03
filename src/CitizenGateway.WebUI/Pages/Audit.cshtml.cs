using CitizenGateway.Contracts.Audit;
using CitizenGateway.WebUI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitizenGateway.WebUI.Pages;

[Authorize(Roles = "Personel")]
public sealed class AuditModel : PageModel
{
    private readonly GatewayApiClient _gateway;

    public AuditModel(GatewayApiClient gateway) => _gateway = gateway;

    public IReadOnlyList<AuditLogDto> Logs { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Logs = await _gateway.GetAuditLogsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
