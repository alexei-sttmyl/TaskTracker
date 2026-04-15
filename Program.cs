using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;
using TaskTracker.Services;

var builder = WebApplication.CreateBuilder(args);

//  Подключение БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

//  Сервисы
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<TaskTracker.Repositories.IHabitRepository, TaskTracker.Repositories.HabitRepository>();
builder.Services.AddScoped<TaskTracker.Services.IHabitService, TaskTracker.Services.HabitService>();
builder.Services.AddScoped<TaskTracker.Services.INotificationService, TaskTracker.Services.NotificationService>();
builder.Services.AddSingleton<TaskTracker.Services.ITimeService, TaskTracker.Services.TimeService>();

// Фоновый сервис напоминаний
builder.Services.AddHostedService<ReminderBackgroundService>();

var app = builder.Build();

//  Middleware
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

app.MapRazorPages();

app.Run();
