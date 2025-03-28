using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data; // Замени на своё пространство имён

namespace MyWebApp.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/Favorites")]
        public IActionResult ShowFavorites()
        {
            // Получаем данные для передачи в представление
            var levels = _context.Levels.ToList();
            var lessonGroups = _context.LessonGroups.ToList();
            var lessons = _context.Lessons.ToList();

            // Передаём данные в представление через ViewBag
            ViewBag.Levels = levels;
            ViewBag.LessonGroups = lessonGroups;
            ViewBag.AllLessons = lessons;

            // Возвращаем представление Favorites.cshtml
            return View("Favorites");
        }
    }
}