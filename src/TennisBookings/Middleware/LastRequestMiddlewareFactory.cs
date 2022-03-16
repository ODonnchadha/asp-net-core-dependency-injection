namespace TennisBookings.Middleware
{
	public class LastRequestMiddlewareFactory : IMiddleware
	{
		private readonly IUtcTimeService _time;
		private readonly UserManager<TennisBookingsUser> _manager;

		public LastRequestMiddlewareFactory(
			IUtcTimeService time,
			UserManager<TennisBookingsUser> manager)
		{
			_time = time;
			_manager = manager;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
			{
				var user = await _manager.FindByNameAsync(context.User.Identity.Name);

				if (user is not null)
				{
					user.LastRequestUtc = _time.CurrentUtcDateTime;
					await _manager.UpdateAsync(user);
				}
			}

			await next(context);
		}
	}
}
