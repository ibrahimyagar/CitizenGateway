using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CitizenGateway.Infrastructure.Persistence;
using CitizenGateway.IntegrationTests.Support;
using FluentAssertions;

namespace CitizenGateway.IntegrationTests;

[Collection(GatewayApiCollection.Name)]
public sealed class AuthAndCitizenEndpointsTests
{
    private readonly GatewayApiFixture _fixture;

    public AuthAndCitizenEndpointsTests(GatewayApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Login_WhenCredentialsValid_ReturnsToken()
    {
        using var client = _fixture.Factory.CreateClient();

        using var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = DbSeeder.PersonelUsername,
            password = DbSeeder.PersonelPassword
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("accessToken").GetString().Should().NotBeNullOrWhiteSpace();
        json.GetProperty("role").GetString().Should().Be("Personel");
    }

    [Fact]
    public async Task GetSummary_WhenNoToken_ReturnsUnauthorized()
    {
        using var client = _fixture.Factory.CreateClient();

        using var response = await client.GetAsync($"/api/citizen/{_fixture.VatandasTcNo}/summary");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSummary_WhenPersonelQueriesValidTc_ReturnsOkShape()
    {
        using var client = _fixture.Factory.CreateClient();
        var token = await AuthTestHelper.LoginAsPersonelAsync(client);
        AuthTestHelper.UseBearer(client, token);

        using var response = await client.GetAsync($"/api/citizen/{_fixture.VatandasTcNo}/summary");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("tcNo").GetString().Should().Be(_fixture.VatandasTcNo);
        json.GetProperty("adSoyad").GetString().Should().NotBeNullOrWhiteSpace();
        json.GetProperty("sporTesisi").ValueKind.Should().Be(JsonValueKind.Object);
        json.GetProperty("kutuphane").ValueKind.Should().Be(JsonValueKind.Object);
        json.GetProperty("cozumMerkezi").ValueKind.Should().Be(JsonValueKind.Object);
        json.GetProperty("partialFailure").GetBoolean().Should().BeFalse();
        json.GetProperty("failedServices").GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task GetSummary_WhenVatandasAccessesOtherTc_ReturnsForbidden()
    {
        using var client = _fixture.Factory.CreateClient();
        var token = await AuthTestHelper.LoginAsVatandasAsync(client);
        AuthTestHelper.UseBearer(client, token);

        using var response = await client.GetAsync($"/api/citizen/{_fixture.OtherTcNo}/summary");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
