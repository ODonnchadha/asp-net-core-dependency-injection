using TennisBookings.Shared.Interfaces.Services;
using TennisBookings.Shared.Services.Weather;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IWeatherForecaster, RandomWeatherForecaster>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/weather/{city}", async (string city, IWeatherForecaster service) =>
{
	var forecast = await service.GetCurrentWeatherAsync(city);

	return forecast.Weather;
});

app.Run();
