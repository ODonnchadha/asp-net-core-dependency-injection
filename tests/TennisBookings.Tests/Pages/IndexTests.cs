using Microsoft.Extensions.Logging.Abstractions;
using TennisBookings.Shared.Interfaces.Services;
using TennisBookings.Shared.Models.Weather;

namespace TennisBookings.Tests.Pages;

public class IndexTests
{
	[Fact]
	public async Task ReturnsExpectedViewModel_WhenWeatherIsMocked()
	{
		var sut = new IndexModel(NullLogger< IndexModel>.Instance, new MockForecaster());

		await sut.OnGet();

		Assert.Contains("We don't have the latest weather information right now, " +
			"please check again later.", sut.WeatherDescription);
	}

	//[Fact]
	//public async Task ReturnsExpectedViewModel_WhenWeatherIsRain()
	//{
	//	//var sut = new IndexModel();

	//	//await sut.OnGet();

	//	//Assert.Contains("We're sorry but it's raining here.", sut.WeatherDescription);
	//}

	private class MockForecaster : IWeatherForecaster
	{
		public Task<WeatherResult> GetCurrentWeatherAsync(string city)
		{
			return Task.FromResult(new WeatherResult
			{
				City = city,
				Weather = new WeatherCondition
				{
					Summary = "MOCK"
				}
			});
		}
	}
}
