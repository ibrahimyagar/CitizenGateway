using Testcontainers.PostgreSql;

namespace CitizenGateway.IntegrationTests.Support;

/// <summary>
/// Her test koleksiyonu için izole PostgreSQL container.
/// Neden Testcontainers? Lokal DB şifresine bağımlı kalmadan CI/lokal aynı şekilde çalışır.
/// </summary>
public sealed class GatewayApiFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("citizen_gateway_it")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public GatewayWebApplicationFactory Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;

    /// <summary>
    /// SharedCitizenCatalog seed=42 ilk kayıt — DbSeeder vatandaş kullanıcısını buna bağlar.
    /// </summary>
    public string VatandasTcNo { get; } = "71151275166";

    /// <summary>Aynı katalogdaki başka bir TC — vatandaş erişememeli.</summary>
    public string OtherTcNo { get; } = "72253325032";

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Factory = new GatewayWebApplicationFactory(_postgres.GetConnectionString());
        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class GatewayApiCollection : ICollectionFixture<GatewayApiFixture>
{
    public const string Name = "GatewayApi";
}
