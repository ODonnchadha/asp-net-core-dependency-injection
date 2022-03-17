using TennisBookings.Caching;
using TennisBookings.Shared.Interfaces.Services;
using TennisBookings.Shared.Models.Weather;

namespace TennisBookings.Services.Weather
{
	public class CachedWeatherForecaster : IWeatherForecaster
	{
		private readonly IWeatherForecaster _weather;
		private readonly IUtcTimeService _time;
		private readonly IDistributedCache<WeatherResult> _cache;

		public CachedWeatherForecaster(
			IWeatherForecaster weather,
			IUtcTimeService time,
			IDistributedCache<WeatherResult> cache)
		{
			_weather = weather;
			_time = time;
			_cache = cache;
		}
		public async Task<WeatherResult> GetCurrentWeatherAsync(string city)
		{
			var cacheKey = $"weather_{city}_{_time.CurrentUtcDateTime:yyy_MM-dd}";

			var (isCached, forecast) = await _cache.TryGetValueAsync(cacheKey);

			if (isCached)
			{
				return forecast!;
			}

			var result = await _weather.GetCurrentWeatherAsync(city);

			await _cache.SetAsync(cacheKey, result, 60);

			return result;
		}
	}
}
