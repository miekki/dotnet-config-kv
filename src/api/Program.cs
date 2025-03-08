using Azure.Identity;
using Microsoft.Extensions.Options;
using api.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

// Add configuration sources based on environment
if (builder.Environment.IsProduction())
{
    var keyVaultUrl = builder.Configuration["KeyVaultConfig:KeyVaultUrl"];
    // builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl!), new DefaultAzureCredential());
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl!),
        new DefaultAzureCredential());
}

// Register settings (works for both local and Key Vault configuration)
builder.Services.Configure<MyAppSettings>(builder.Configuration.GetSection("MyAppSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/mysettings", (IOptions<MyAppSettings> options) =>
{
    var settings = options.Value;
    return Results.Ok(settings);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
