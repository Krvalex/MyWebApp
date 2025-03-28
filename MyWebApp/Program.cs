using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы MVC
builder.Services.AddControllersWithViews();

// Задаем строку подключения прямо в коде
var connectionString = "Host=tramway.proxy.rlwy.net;Port=51907;Database=railway;Username=postgres;Password=BwgjLIdcvLdgyevibKXDosEgzRnXWWvb";

// Настраиваем DbContext для использования PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Настраиваем конвейер обработки запросов
app.UseStaticFiles();
app.UseRouting();

// Настраиваем маршруты для MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();