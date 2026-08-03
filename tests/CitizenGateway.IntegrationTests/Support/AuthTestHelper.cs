using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CitizenGateway.Domain.Enums;
using CitizenGateway.Infrastructure.Persistence;

namespace CitizenGateway.IntegrationTests.Support;

internal static class AuthTestHelper
{
    public static async Task<string> LoginAsync(
        HttpClient client,
        LoginPortal portal,
        string identifier,
        string password)
    {
        using var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            portal = portal.ToString(),
            identifier,
            password
        });

        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        return doc.RootElement.GetProperty("accessToken").GetString()
            ?? throw new InvalidOperationException("accessToken yok.");
    }

    public static async Task<string> LoginAsPersonelAsync(HttpClient client) =>
        await LoginAsync(client, LoginPortal.Personel, DbSeeder.PersonelEmail, DbSeeder.PersonelPassword);

    public static async Task<string> LoginAsVatandasAsync(HttpClient client) =>
        await LoginAsync(client, LoginPortal.Vatandas, DbSeeder.VatandasUsername, DbSeeder.VatandasPassword);

    public static void UseBearer(HttpClient client, string token) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}
