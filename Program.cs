var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => Results.Ok(new
{
    status = "ok",
    service = "dio-weather-api",
    message = "API no ar! Consulte /api/weatherforecast"
}));

app.MapGet("/api/weatherforecast", () =>
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
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    // Fahrenheit e Kelvin calculados a partir da temperatura em Celsius,
    // conforme demonstrado no vídeo do desafio (adição feita "ao vivo" com apoio do Copilot).
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public double TemperatureK => Math.Round(TemperatureC + 273.15, 2);
}
