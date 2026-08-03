using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CitizenGateway.WebUI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitizenGateway.WebUI.Pages;

public sealed class LoginModel : PageModel
{
    private readonly GatewayApiClient _gateway;

    public LoginModel(GatewayApiClient gateway) => _gateway = gateway;

    [BindProperty]
    [Required]
    public string Username { get; set; } = "personel";

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "Personel123!";

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
            var login = await _gateway.LoginAsync(Username.Trim(), Password, cancellationToken);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, login.Username),
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
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
