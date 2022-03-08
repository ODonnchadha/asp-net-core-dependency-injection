
using TennisBookings.Shared.Models.Weather;

namespace TennisBookings.Shared.Interfaces.Services
{
	public interface IWeatherForecaster
	{
		Task<WeatherResult> GetCurrentWeatherAsync(string city);
	}
}
