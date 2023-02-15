using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBusiness.Entities
{
	public class PipelineDTO: TestRedisDTO
	{
		public string Unpipelined { get; set; }
		public string AutomaticallyPipelined { get; set; }
		public string BatchedPipelined { get; set; }
	}
}
