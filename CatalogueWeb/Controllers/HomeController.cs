using CatalogueWeb.Models;
using Database;
using Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;

namespace CatalogueWeb.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly TestRedisContext _testRedisContext;
		private readonly IDistributedCache _cache;
		public HomeController(ILogger<HomeController> logger, TestRedisContext testRedisContext, IDistributedCache cache)
		{
			_logger = logger;
			_testRedisContext = testRedisContext;
			_cache = cache;

		}

		public async Task<IActionResult> Index()
		{
			//Guasrda un empleado aleatorio
			_ = _testRedisContext.Employees.Add(new Employee
			{
				Name = $"Employe_{Guid.NewGuid().ToString()}",
				Sales = { new Sale
				{
					Total = new Random().Next(1,99999)
				} }
			});

			_ = await _testRedisContext.SaveChangesAsync().ConfigureAwait(false);

			//consulta el top de ventas de cache
			var topSalesTask = _cache.GetStringAsync("top:sales");
			var topNameTask = _cache.GetStringAsync("top:name");

			await Task.WhenAll(topSalesTask, topNameTask);

			if (string.IsNullOrEmpty(topSalesTask.Result) && string.IsNullOrEmpty(topNameTask.Result))
			{
				Trace.TraceInformation(topSalesTask.Result);
				Trace.TraceInformation(topNameTask.Result);

				//Setear cache de top de ventas 
				var topSalesperson = await _testRedisContext.Employees.Select(x => new {
						Employee = x,
						sumSales = x.Sales
							.Sum(x => x.Total)
					}).OrderByDescending(x => x.sumSales)
					.FirstAsync();

				var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
				var topSalesInsertTask = _cache.SetStringAsync("top:sales", topSalesperson.sumSales.ToString(), cacheOptions);
				var topNameInsertTask = _cache.SetStringAsync("top:name", topSalesperson.Employee.Name, cacheOptions);
				await Task.WhenAll(topSalesInsertTask, topNameInsertTask);
			}

			var id = 12;
			var algo = await _testRedisContext.Employees.Include(x => x.Sales).ToListAsync();

			var avg = await _testRedisContext.Employees.Include(x => x.Sales).Where(x => x.EmployeeId == id).Select(x => x.Sales.Average(y => y.Total)).FirstAsync();

			var key = $"employee:{id}:avg";
			var cacheResult = await _cache.GetStringAsync(key);

			if (cacheResult == null)
			{
				await _cache.SetStringAsync(key, avg.ToString(CultureInfo.InvariantCulture), options: new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(30) });
			}







			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}