using TennisBookings.Shared.Interfaces.Services;
using TennisBookings.Shared.Models.Weather;

namespace TennisBookings.Shared.Services.Weather
{
	public class WeatherForecaster : IWeatherForecaster
	{
		public Task<WeatherResult> GetCurrentWeatherAsync(string city)
		{
			return Task.FromResult(
				new WeatherResult
				{
					City = city,
					Weather = new WeatherCondition
					{
						Summary = "Cloudy"
					}
				}
			);
		}
	}
}
