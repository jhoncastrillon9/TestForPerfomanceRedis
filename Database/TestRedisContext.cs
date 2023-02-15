using Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
	public class TestRedisContext : DbContext
	{
		public TestRedisContext(DbContextOptions<TestRedisContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Sale>()
				.HasOne(p => p.Employee)
				.WithMany(o => o.Sales)
				.HasForeignKey(p => p.EmployeeId);

		}


		public DbSet<Employee> Employees { get; set; }
		public DbSet<Sale> Sales { get; set; }
	}
}
