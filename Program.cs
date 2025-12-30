using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloCSharp.Data;
using HelloCSharp.Areas.UserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=HelloCSharp.db"));

// Add Application Services
builder.Services.AddScoped<IAttributeService, AttributeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserAttributeValueService, UserAttributeValueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// (Authentication can be added later) Authorization placeholder
app.UseAuthorization();

// Area routing
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
