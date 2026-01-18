using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloCSharp.Data;
using HelloCSharp.Services;
using HelloCSharp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=HelloCSharp.db"));

// Add Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttributeRepository, AttributeRepository>();
builder.Services.AddScoped<IUserAttributeValueRepository, UserAttributeValueRepository>();

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

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
