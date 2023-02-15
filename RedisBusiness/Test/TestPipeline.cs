using RedisBusiness.Entities;
using System.Diagnostics;
using RedisBusiness.Enums;

namespace RedisBusiness.Test
{
	public class TestPipeline : BaseRedis<PipelineDTO>
	{
		public override async Task<PipelineDTO> ExecuteTest()
		{
			_ = new PipelineDTO();

			try
			{

				var testRedis = new PipelineDTO
				{
					EndPointCollection = $"EndPointRedis: {_endPointRedis}",
					Ping = $"ping: {_db.Ping().TotalMilliseconds} ms",
					Database = $"Db: {_db.Database}"
				};

				var stopwatch = Stopwatch.StartNew();

				// un-pipelined commands incur the added cost of an extra round trip
				for (var i = 0; i < 1000; i++)
				{
					_ = await _db.PingAsync();
				}

				testRedis.Unpipelined =
					$"1000 un-pipelined commands took: {stopwatch.ElapsedMilliseconds}ms to execute";

				var pingTasks = new List<Task<TimeSpan>>();

				// restart stopwatch
				stopwatch.Restart();

				for (var i = 0; i < 1000; i++)
				{
					pingTasks.Add(_db.PingAsync());
				}

				_ = await Task.WhenAll(pingTasks);

				testRedis.AutomaticallyPipelined = $"1000 automatically pipelined tasks took: {stopwatch.ElapsedMilliseconds}ms to execute, first result: {pingTasks[0].Result}";

				// clear our ping tasks list.
				pingTasks = new List<Task<TimeSpan>>();

				// Batches allow you to more intentionally group together the commands that you want to send to Redis.
				// If you employee a batch, all commands in the batch will be sent to Redis in one contiguous block, with no
				// other commands from the client interleaved. Of course, if there are other clients to Redis, commands from those
				// other clients may be interleaved with your batched commands.
				var batch = _db.CreateBatch();

				// restart stopwatch
				stopwatch.Restart();

				for (var i = 0; i < 1000; i++)
				{
					pingTasks.Add(batch.PingAsync());
				}

				batch.Execute();
				_ = await Task.WhenAll(pingTasks);
				testRedis.BatchedPipelined = $"1000 batched commands took: {stopwatch.ElapsedMilliseconds}ms to execute, first result: {pingTasks[0].Result}";
				return testRedis;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public override Task<PipelineDTO> AddKeys(int quantity = 100, PipelineEnum pipeline = PipelineEnum.UnPipelined)
		{
			throw new NotImplementedException();
		}
	}
}
