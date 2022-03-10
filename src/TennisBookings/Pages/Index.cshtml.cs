using Microsoft.AspNetCore.Mvc.RazorPages;
using TennisBookings.Shared.Interfaces.Services;

namespace TennisBookings.Pages
{
	public class IndexModel : PageModel
    {
		// This is registered as part of the ASP.NET Core Hosting Framework.
		private readonly ILogger<IndexModel> logger;
		private readonly IWeatherForecaster service;
		public IndexModel(ILogger<IndexModel> logger, IWeatherForecaster service)
		{
			this.logger = logger;
			this.service = service;
		}
		public string WeatherDescription { get; private set; } =
            "We don't have the latest weather information right now, " +
			"please check again later.";
        public bool ShowWeatherForecast => true;
        public bool ShowGreeting => false;
        public string Greeting => "Welcome to Tennis by the Sea";

        public async Task OnGet()
        {
			// NOTES:
			// This is a hard dependency. Creation of the instance with 'new.'
			// Tight coupling. Code is hard to maintain over time.
			// Do not directly couple two classes together.
			// var forecaster = new RandomWeatherForecaster();

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
