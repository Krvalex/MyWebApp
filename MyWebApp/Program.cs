using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using Npgsql; // Для работы с NpgsqlConnectionStringBuilder

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы MVC
builder.Services.AddControllersWithViews();

// Получаем строку подключения из appsettings.json или переменной окружения DATABASE_URL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

// Если строка подключения в формате URI, преобразуем её в формат ключ-значение
if (connectionString.StartsWith("postgresql://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]}";
}

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