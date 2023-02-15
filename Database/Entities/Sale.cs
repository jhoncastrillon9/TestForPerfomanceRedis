using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities
{ public class Sale
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SaleId { get; set; }
		public int Total { get; set; }

		public int EmployeeId { get; set; }
		public Employee Employee { get; set; }
	}
}
