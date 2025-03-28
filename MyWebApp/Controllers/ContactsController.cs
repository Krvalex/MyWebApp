using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Controllers // Замени MyWebApp на имя твоего проекта
{
    public class ContactsController : Controller
    {
        [HttpGet("/contacts")]
        public IActionResult Index()
        {
            return View();
        }
    }
}