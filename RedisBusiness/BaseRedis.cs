using RedisBusiness.Entities;
using RedisBusiness.Enums;
using StackExchange.Redis;

namespace RedisBusiness
{
	public abstract class BaseRedis<T>
	{
		protected ConnectionMultiplexer _muxer;
		protected IDatabase _db;
		protected string _endPointRedis;

		public BaseRedis(string endPointRedis = "localhost:6379")
		{
			_endPointRedis = endPointRedis;
			var options = new ConfigurationOptions
			{
				EndPoints = new EndPointCollection { endPointRedis }
			};

			_muxer = ConnectionMultiplexer.Connect(options);

			_db = _muxer.GetDatabase();
		}

		public abstract Task<T> ExecuteTest();
		public abstract Task<T> AddKeys(int quantity = 100, PipelineEnum pipeline = PipelineEnum.UnPipelined);
	}
}
