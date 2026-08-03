using CozumMerkezi.Api.Data;

// SENTETİK VERİ UYARISI: Gerçek çözüm merkezi / 153 hattına bağlantı yoktur.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "CozumMerkezi Mock API",
        Description = "Örnekköy Belediyesi çözüm merkezi simülasyonu — tamamen sentetik veri."
    });
});

builder.Services.AddSingleton<CozumMerkeziSeedStore>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "CozumMerkezi.Api", status = "Healthy" }));

app.Run();
