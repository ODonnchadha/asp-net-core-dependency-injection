namespace ServiceLifetimeDemonstration
{
	public class DisposableService : IDisposable, IAsyncDisposable
	{
		private readonly ILogger<DisposableService> logger;
		public DisposableService(ILogger<DisposableService> logger) => this.logger = logger;
		public void Dispose() => logger.LogInformation("Disposing of service.");
		public ValueTask DisposeAsync()
		{
			logger.LogInformation("Disposing of service.");

			return ValueTask.CompletedTask;
		}
	}
}
