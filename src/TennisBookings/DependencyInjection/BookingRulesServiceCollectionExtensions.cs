namespace TennisBookings.DependencyInjection
{
	public static class BookingRulesServiceCollectionExtensions
	{
		public static IServiceCollection AddBookingRules(this IServiceCollection services)
		{
			//// Use Add() and not TryAdd() because we want all implementations:
			//services.AddSingleton<ICourtBookingRule, ClubIsOpenRule>();
			//services.AddSingleton<ICourtBookingRule, MaxBookingLengthRule>();
			//services.AddSingleton<ICourtBookingRule, MaxPeakTimeBookingLengthRule>();
			//services.AddScoped<ICourtBookingRule, MemberBookingsMustNotOverlapRule>();
			//services.AddScoped<ICourtBookingRule, MemberCourtBookingsMaxHoursPerDayRule>();

			// Assembly scanning. Assign rules dynamically.
			services.Scan(scan =>
				scan.FromAssemblyOf<ICourtBookingRule>().AddClasses(c =>
				c.AssignableTo<ICourtBookingRule>()).AsImplementedInterfaces().WithScopedLifetime());

			services.AddScoped<IBookingRuleProcessor, BookingRuleProcessor>();

			return services;
		}
	}
}
