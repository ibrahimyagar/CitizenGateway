using SporTesisi.Api.Data;

// SENTETİK VERİ UYARISI: Bu servis gerçek belediye/spor sistemi değildir.
// Tüm yanıtlar Bogus ile üretilmiş sahte kayıtlardır; gerçek kişi verisi yoktur.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "SporTesisi Mock API",
        Description = "Örnekköy Belediyesi spor tesisi simülasyonu — tamamen sentetik veri."
    });
});

// Singleton: seed bir kez üretilir, tüm istekler aynı katalogu görür.
builder.Services.AddSingleton<SporTesisiSeedStore>();

var app = builder.Build();

// Demo'da Swagger her ortamda açık — keşif kolaylığı için.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "SporTesisi.Api", status = "Healthy" }));

app.Run();
