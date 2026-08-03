using System.ComponentModel.DataAnnotations;
using CitizenGateway.Contracts.Citizens;
using CitizenGateway.Contracts.Health;
using CitizenGateway.Contracts.Requests;
using CitizenGateway.Domain.Enums;
using CitizenGateway.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CitizenGateway.WebUI.Pages;

public sealed class IndexModel : PageModel
{
    private readonly GatewayApiClient _gateway;
    private readonly CitizenWorkspaceLoader _workspace;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        GatewayApiClient gateway,
        CitizenWorkspaceLoader workspace,
        ILogger<IndexModel> logger)
    {
        _gateway = gateway;
        _workspace = workspace;
        _logger = logger;
    }

    public bool IsPersonel { get; private set; }
    public string Username => User.Identity?.Name ?? "";
    public GatewayHealthDto Health { get; private set; } = new();
    public List<SelectListItem> CitizenOptions { get; private set; } = [];
    public CitizenSummaryDto? Summary { get; private set; }
    public IReadOnlyList<ServiceRequestDto> Requests { get; private set; } = [];
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }

    [BindProperty]
    [Display(Name = "T.C. Kimlik No")]
    public string TcNo { get; set; } = "";

    [BindProperty]
    [Display(Name = "Talep türü")]
    public RequestType NewRequestType { get; set; } = RequestType.KursKaydi;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var shell = await _workspace.LoadShellAsync(User, cancellationToken);
        ApplyShell(shell);
        TcNo = _workspace.ResolveDefaultTc(shell, TcNo);
    }

    public async Task<IActionResult> OnPostQueryAsync(CancellationToken cancellationToken)
    {
        var shell = await _workspace.LoadShellAsync(User, cancellationToken);
        ApplyShell(shell);

        if (!TryAuthorizeTc(shell, out var error))
        {
            ErrorMessage = error;
            return Page();
        }

        try
        {
            var data = await _workspace.LoadCitizenAsync(TcNo.Trim(), cancellationToken);
            Summary = data.Summary;
            Requests = data.Requests;
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
        var shell = await _workspace.LoadShellAsync(User, cancellationToken);
        ApplyShell(shell);

        if (!TryAuthorizeTc(shell, out var error))
        {
            ErrorMessage = error;
            return Page();
        }

        try
        {
            await _gateway.CreateRequestAsync(TcNo.Trim(), NewRequestType, cancellationToken);
            SuccessMessage = "Talep oluşturuldu.";
            var data = await _workspace.LoadCitizenAsync(TcNo.Trim(), cancellationToken);
            Summary = data.Summary;
            Requests = data.Requests;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Talep oluşturma başarısız");
            ErrorMessage = ex.Message;
            try
            {
                var data = await _workspace.LoadCitizenAsync(TcNo.Trim(), cancellationToken);
                Summary = data.Summary;
                Requests = data.Requests;
            }
            catch
            {
                // ignore secondary load failure
            }
        }

        return Page();
    }

    private void ApplyShell(WorkspaceShell shell)
    {
        Health = shell.Health;
        IsPersonel = shell.IsPersonel;
        CitizenOptions = shell.CitizenOptions;
    }

    private bool TryAuthorizeTc(WorkspaceShell shell, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(TcNo))
        {
            error = "TC seçin veya girin.";
            return false;
        }

        if (!shell.IsPersonel &&
            !string.Equals(TcNo, shell.LinkedTcNo, StringComparison.Ordinal))
        {
            error = "Vatandaş yalnızca kendi TC'sini sorgulayabilir.";
            TcNo = shell.LinkedTcNo ?? TcNo;
            return false;
        }

        return true;
    }
}
