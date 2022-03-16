namespace TennisBookings.Middleware
{
	public class LastRequestMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IUtcTimeService _time;
		// private readonly UserManager<TennisBookingsUser> _manager;

		public LastRequestMiddleware(
			RequestDelegate next,
			IUtcTimeService time)
		{
			_next = next;
			_time = time;
			//_manager = manager;
		}

		public async Task InvokeAsync(HttpContext context, UserManager<TennisBookingsUser> manager)
		{
			if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
			{
				var user = await manager.FindByNameAsync(context.User.Identity.Name);

				if (user is not null)
				{
					user.LastRequestUtc = _time.CurrentUtcDateTime;
					await manager.UpdateAsync(user);
				}
			}

			await _next(context);
		}
	}
}
