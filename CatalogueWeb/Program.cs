using CatalogueWeb;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TestRedisContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddHttpContextAccessor();

// Agregar la configuración de la caché de Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.ConfigurationOptions = new ConfigurationOptions
	{
		EndPoints = { "localhost:6379" },
		Password = ""
	};
});

builder.Services.AddHostedService<InitService>();

var app = builder.Build();


using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<TestRedisContext>();
await dbContext.Database.EnsureCreatedAsync();


if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
