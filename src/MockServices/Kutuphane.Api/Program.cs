using Kutuphane.Api.Data;

// SENTETİK VERİ UYARISI: Gerçek kütüphane sistemine bağlantı yoktur.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Kutuphane Mock API",
        Description = "Örnekköy Belediyesi kütüphane simülasyonu — tamamen sentetik veri."
    });
});

builder.Services.AddSingleton<KutuphaneSeedStore>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { service = "Kutuphane.Api", status = "Healthy" }));

app.Run();
