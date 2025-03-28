using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Controllers // Замени MyWebApp на имя твоего проекта
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
