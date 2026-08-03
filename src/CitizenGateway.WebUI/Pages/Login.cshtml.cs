using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CitizenGateway.Domain.Enums;
using CitizenGateway.WebUI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitizenGateway.WebUI.Pages;

public sealed class LoginModel : PageModel
{
    private readonly GatewayApiClient _gateway;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(GatewayApiClient gateway, ILogger<LoginModel> logger)
    {
        _gateway = gateway;
        _logger = logger;
    }

    [BindProperty]
    public LoginPortal Portal { get; set; } = LoginPortal.Personel;

    [BindProperty]
    [Required(ErrorMessage = "Bu alan zorunludur.")]
    [Display(Name = "Kimlik")]
    public string Identifier { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "Şifre gerekli.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = "";

    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var login = await _gateway.LoginAsync(Portal, Identifier.Trim(), Password, cancellationToken);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, login.DisplayName),
                new(ClaimTypes.Role, login.Role),
                new(WebUiClaimTypes.AccessToken, login.AccessToken)
            };

            if (!string.IsNullOrWhiteSpace(login.LinkedCitizenTcNo))
                claims.Add(new Claim(WebUiClaimTypes.LinkedTcNo, login.LinkedCitizenTcNo));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Giriş başarısız. Portal={Portal}", Portal);
            ErrorMessage = "Giriş başarısız. Bilgilerinizi kontrol edin.";
            return Page();
        }
    }
}
