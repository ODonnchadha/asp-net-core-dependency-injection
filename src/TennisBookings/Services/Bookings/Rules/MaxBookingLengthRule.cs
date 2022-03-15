using Microsoft.Extensions.Options;

namespace TennisBookings.Services.Bookings.Rules
{
	public class MaxBookingLengthRule : ICourtBookingRule
	{
		//private readonly BookingConfiguration _bookingConfiguration;

		//public MaxBookingLengthRule(IOptions<BookingConfiguration> options)
		//{
		//	_bookingConfiguration = options.Value;
		//}

		private readonly IBookingConfiguration _configuration;
		public MaxBookingLengthRule(IBookingConfiguration configuration)
		{
			_configuration = configuration;
		}

		public Task<bool> CompliesWithRuleAsync(CourtBooking booking)
		{
			var bookingLength = booking.EndDateTime - booking.StartDateTime;

			// var compliesWithRule = bookingLength <= TimeSpan.FromHours(_bookingConfiguration.MaxRegularBookingLengthInHours);
			var compliesWithRule = bookingLength <= TimeSpan.FromHours(_configuration.MaxRegularBookingLengthInHours);

			return Task.FromResult(compliesWithRule);
		}

		public string ErrorMessage => "Booking is longer than allowed booking length";
	}
}
