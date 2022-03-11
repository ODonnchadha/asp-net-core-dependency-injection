using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TennisBookings.Configuration;
using TennisBookings.Shared.Interfaces.Services;
using TennisBookings.Shared.Models.Weather;

namespace TennisBookings.Tests.Pages;

public class IndexTests
{
	[Fact]
	public async Task ReturnsExpectedViewModel_WhenWeatherIsMocked()
	{
		var sut = new IndexModel(
			new EnabledConfiguration(),
			NullLogger<IndexModel>.Instance,
			new MockForecaster());

		await sut.OnGet();

		Assert.Contains("We don't have the latest weather information right now, " +
			"please check again later.", sut.WeatherDescription);
	}

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

	private class EnabledConfiguration: IOptionsSnapshot<FeaturesConfiguration>
	{
		public FeaturesConfiguration Value => new FeaturesConfiguration { EnableWeatherForecast = true };
		public FeaturesConfiguration Get(string name) => throw new NotImplementedException();
	}
}
