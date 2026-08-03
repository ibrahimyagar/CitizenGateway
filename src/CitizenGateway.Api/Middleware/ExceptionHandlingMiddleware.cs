using CitizenGateway.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace CitizenGateway.Api.Middleware;

/// <summary>
/// Domain exception → HTTP status eşlemesi.
/// Controller'larda try/catch şişmesin diye pipeline'da merkezi yakalama.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await WriteErrorAsync(context, ex);
        }
    }

    private async Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            InvalidTcNoException => (HttpStatusCode.BadRequest, "Geçersiz TC"),
            DomainValidationException => (HttpStatusCode.BadRequest, "Doğrulama hatası"),
            UnauthorizedCitizenAccessException => (HttpStatusCode.Forbidden, "Yetkisiz erişim"),
            CitizenNotFoundException => (HttpStatusCode.NotFound, "Vatandaş bulunamadı"),
            _ => (HttpStatusCode.InternalServerError, "Sunucu hatası")
        };

        if (status == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "İşlenmeyen hata");
        else
            _logger.LogWarning(ex, "İş kuralı/erişim hatası: {Title}", title);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var payload = new
        {
            title,
            detail = status == HttpStatusCode.InternalServerError
                ? "Beklenmeyen bir hata oluştu."
                : ex.Message,
            status = (int)status
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }
}
