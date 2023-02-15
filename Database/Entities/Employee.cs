using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities
{
	public class Employee
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int EmployeeId { get; set; }
		public string Name { get; set; }
		public List<Sale> Sales { get; } = new();
	}
}
