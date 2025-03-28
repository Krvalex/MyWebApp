using Microsoft.EntityFrameworkCore;
using MyWebApp.Data; // ������ MyWebApp �� ��� ������ �������

var builder = WebApplication.CreateBuilder(args);

// ��������� ��������� MVC
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// �������� ��������� ����������� ������ (CSS, JS, ��������)
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();