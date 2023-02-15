using Database;
using Microsoft.Extensions.Caching.Distributed;

namespace CatalogueWeb
{
	public class InitService : IHostedService
	{
		private readonly IServiceScopeFactory _scopeFactory;

		public InitService(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory;
		}
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _scopeFactory.CreateScope();
			var salesDb = scope.ServiceProvider.GetRequiredService<TestRedisContext>();

			
			// add cache invalidation logic here.
			var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
			// End Section 3.2 Step 2

			var cachePipe = new List<Task>
			{
				cache.RemoveAsync("top:sales", cancellationToken),
				cache.RemoveAsync("top:name", cancellationToken),
				cache.RemoveAsync("totalSales", cancellationToken)
			};

			cachePipe.AddRange(salesDb.Employees.Select(employee => cache.RemoveAsync($"employee:{employee.EmployeeId}:avg", cancellationToken)));

			await Task.WhenAll(cachePipe);

		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
