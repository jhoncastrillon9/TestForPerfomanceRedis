using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBusiness.Entities
{
	public class StringDTO:TestRedisDTO
	{
		public List<String> Instructors { get; set; } = new List<String>();
		public string TimeSeconds { get; set; }
	}
}
