using Microsoft.VisualBasic;
using RedisBusiness.Entities;
using RedisBusiness.Enums;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RedisBusiness.Test
{
	public class TestString:BaseRedis<StringDTO>
	{
		public override async Task<StringDTO> ExecuteTest()
		{
			var testStringDTO = await AddKeys();

			return testStringDTO;
		}

		public override async Task<StringDTO> AddKeys(int quantity = 100, PipelineEnum pipeline = PipelineEnum.UnPipelined)
		{
		
			var testStringDTO = new StringDTO();

			switch (pipeline)
			{
				case PipelineEnum.PipelinedTasks:
					await SetAndGetKeysPipelineTask(quantity, testStringDTO);
					return testStringDTO;
					break;
				case PipelineEnum.BatchedCommands:
					await SetAndGetKeysPipelineBacth(quantity, testStringDTO);
					return testStringDTO;
					break;

				case PipelineEnum.UnPipelined:
					SetAndGetKeysUnpipeline(quantity, testStringDTO);
					return testStringDTO;
					break;
			}

			return testStringDTO;
		}

		private void SetAndGetKeysUnpipeline(int quantity, StringDTO testStringDTO)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var salt = Guid.NewGuid().ToString();

			for (int i = 1; i <= quantity; i++)
			{
				var instructorNameKey = new RedisKey($"instructors:{salt}_{i}:name");

				_db.StringSet(instructorNameKey, $"Instructor Name {salt}_{i}");
				var instructor1Name = _db.StringGet(instructorNameKey);
				testStringDTO.Instructors.Add($"{instructor1Name}");
			}

			stopwatch.Stop();
			testStringDTO.TimeSeconds = stopwatch.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
		}



		private async Task SetAndGetKeysPipelineTask(int quantity, StringDTO testStringDTO)
		{
			var salt = Guid.NewGuid().ToString();
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var tasks = new List<Task>();
			for (int i = 1; i <= quantity; i++)
			{
				var instructorNameKey = new RedisKey($"instructors:{salt}_{i}:name");
				tasks.Add(_db.StringSetAsync(instructorNameKey, $"Instructor Name {salt}_{i}"));
				tasks.Add(_db.StringGetAsync(instructorNameKey)
					.ContinueWith(x => testStringDTO.Instructors.Add($"{x.Result}")));
			}

			await Task.WhenAll(tasks);

			stopwatch.Stop();
			testStringDTO.TimeSeconds = stopwatch.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
		}

		private async Task SetAndGetKeysPipelineBacth(int quantity, StringDTO testStringDTO)
		{
			var salt = Guid.NewGuid().ToString();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var batch = _db.CreateBatch();
			var tasks = new List<Task>();
			for (int i = 1; i <= quantity; i++)
			{
				var instructorNameKey = new RedisKey($"instructors:{salt}_{i}:name");
				tasks.Add(batch.StringSetAsync(instructorNameKey, $"Instructor Name {salt}_{i}"));
				tasks.Add(batch.StringGetAsync(instructorNameKey)
					.ContinueWith(x => testStringDTO.Instructors.Add($"{x.Result}")));
			}

			batch.Execute();
			await Task.WhenAll(tasks);
			stopwatch.Stop();
			testStringDTO.TimeSeconds = stopwatch.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
		}
	}
}
