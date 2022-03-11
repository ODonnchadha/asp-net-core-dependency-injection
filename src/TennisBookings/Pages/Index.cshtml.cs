using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using TennisBookings.Shared.Interfaces.Services;

namespace TennisBookings.Pages
{
	public class IndexModel : PageModel
    {
		private readonly FeaturesConfiguration configuration;
		private readonly ILogger<IndexModel> logger;
		private readonly IWeatherForecaster service;
		public IndexModel(
			IOptionsSnapshot<FeaturesConfiguration> configuration,
			ILogger<IndexModel> logger,
			IWeatherForecaster service)
		{
			this.configuration = configuration.Value;
			this.logger = logger;
			this.service = service;
		}
		public string WeatherDescription { get; private set; } =
            "We don't have the latest weather information right now, " +
			"please check again later.";
        public bool ShowWeatherForecast { get; private set; }
        public bool ShowGreeting => false;
        public string Greeting => "Welcome to Tennis by the Sea";

        public async Task OnGet()
        {
			ShowWeatherForecast = configuration.EnableWeatherForecast;

			if (ShowWeatherForecast)
			{
				try
				{
					var currentWeather = await service.GetCurrentWeatherAsync("Eastbourne");

					switch (currentWeather.Weather.Summary)
					{
						case "Sun":
							WeatherDescription = "It's sunny right now. " +
								"A great day for tennis!";
							break;

						case "Cloud":
							WeatherDescription = "It's cloudy at the moment " +
								"and the outdoor courts are in use.";
							break;

						case "Rain":
							WeatherDescription = "We're sorry but it's raining here. " +
								"No outdoor courts in use.";
							break;

						case "Snow":
							WeatherDescription = "It's snowing!! Outdoor courts will " +
								"remain closed until the snow has cleared.";
							break;
					}
				}
				catch (Exception ex)
				{
					logger.LogError(ex?.Message);
				}
			}
        }
    }
}
