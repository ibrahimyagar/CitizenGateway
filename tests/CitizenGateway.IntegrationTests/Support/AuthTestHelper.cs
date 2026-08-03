using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CitizenGateway.Infrastructure.Persistence;

namespace CitizenGateway.IntegrationTests.Support;

internal static class AuthTestHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<string> LoginAsync(HttpClient client, string username, string password)
    {
        using var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username,
            password
        });

        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        return doc.RootElement.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("accessToken yok.");
    }

    public static async Task<string> LoginAsPersonelAsync(HttpClient client) =>
        await LoginAsync(client, DbSeeder.PersonelUsername, DbSeeder.PersonelPassword);

    public static async Task<string> LoginAsVatandasAsync(HttpClient client) =>
        await LoginAsync(client, DbSeeder.VatandasUsername, DbSeeder.VatandasPassword);

    public static void UseBearer(HttpClient client, string token) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}
