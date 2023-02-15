using Microsoft.AspNetCore.Mvc;
using RedisBusiness.Entities;
using RedisBusiness.Enums;
using RedisBusiness.Test;

namespace CatalogueWeb.Controllers
{
    public class TestRedisController : Controller
    {
        public async Task<IActionResult> Index()
        {
            _ = new PipelineDTO();
            PipelineDTO? testRedis;
            try
            {
	            var testPipeline = new TestPipeline();
	            testRedis = await testPipeline.ExecuteTest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return View(testRedis);
        }

		public async Task<IActionResult> TestString()
		{
			_ = new StringDTO();
			StringDTO? testRedis;
			try
			{
				var testPipeline = new TestString();
				testRedis = await testPipeline.ExecuteTest();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			return View(testRedis);
		}

		public async Task<JsonResult> AddKeys(int quantity = 100, int pipeline = 0)
		{
			_ = new StringDTO();
			StringDTO? testRedis;
			try
			{
				PipelineEnum pipelineEnum = (PipelineEnum)pipeline;
				var testString = new TestString();
				testRedis = await testString.AddKeys(quantity, pipelineEnum);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			return Json(new { instructors = testRedis.Instructors, timeSeconds = testRedis.TimeSeconds });
		}
	}
}
