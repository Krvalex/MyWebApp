using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyWebApp.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext _context;
        private const int INITIAL_LESSONS_PER_GROUP = 2;

        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/catalog")]
        public IActionResult ShowCatalog()
        {
            var levels = _context.Levels.ToList();
            var lessonGroups = _context.LessonGroups.ToList();
            var lessons = _context.Lessons.ToList();

            // Группируем уроки по уровням и группам, берем только первые 2 урока для каждой группы
            var lessonsByLevelAndGroup = lessons
                .GroupBy(l => l.LevelId)
                .ToDictionary(
                    lg => lg.Key,
                    lg => lg
                        .GroupBy(l => l.GroupId)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Take(INITIAL_LESSONS_PER_GROUP).ToList()
                        )
                );

            ViewData["levels"] = levels;
            ViewData["lessonGroups"] = lessonGroups;
            ViewData["lessonsByLevelAndGroup"] = lessonsByLevelAndGroup;
            ViewData["allLessons"] = lessons; // Для клиентской фильтрации

            return View("Catalog");
        }

        [HttpGet("/catalog/filter")]
        public IActionResult FilterLessons(string levelName, string dateFilter)
        {
            var lessonsQuery = _context.Lessons
                .Include(l => l.Level)
                .Include(l => l.Group) // Исправлено: используем Group вместо LessonGroup
                .AsQueryable();

            // Фильтрация по уровню
            if (!string.IsNullOrEmpty(levelName) && levelName != "all")
            {
                lessonsQuery = lessonsQuery.Where(l => l.Level.Name == levelName);
            }

            // Фильтрация по дате
            if (!string.IsNullOrEmpty(dateFilter) && dateFilter != "all")
            {
                var now = DateTime.UtcNow;
                switch (dateFilter)
                {
                    case "new":
                        lessonsQuery = lessonsQuery.Where(l => l.ReleaseDate >= now.AddDays(-3));
                        break;
                    case "week":
                        lessonsQuery = lessonsQuery.Where(l => l.ReleaseDate >= now.AddDays(-7));
                        break;
                    case "month":
                        lessonsQuery = lessonsQuery.Where(l => l.ReleaseDate >= now.AddMonths(-1));
                        break;
                }
            }

            var lessons = lessonsQuery.ToList();

            // Группируем уроки по уровням и группам
            var lessonsByLevelAndGroup = lessons
                .GroupBy(l => l.LevelId)
                .ToDictionary(
                    lg => lg.Key,
                    lg => lg
                        .GroupBy(l => l.GroupId)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Take(INITIAL_LESSONS_PER_GROUP).ToList()
                        )
                );

            var levels = _context.Levels.ToList();
            var lessonGroups = _context.LessonGroups.ToList();

            // Формируем данные для ответа
            var response = new
            {
                levels = levels,
                lessonGroups = lessonGroups,
                lessonsByLevelAndGroup = lessonsByLevelAndGroup
            };

            return Json(response);
        }

        [HttpGet("/catalog/search-groups")]
        public IActionResult SearchLessonGroups(string query)
        {
            try
            {
                // Получаем все группы уроков из базы данных
                var groups = _context.LessonGroups.AsQueryable();

                // Если пользователь ввёл текст для поиска, фильтруем группы по названию (регистронезависимо)
                if (!string.IsNullOrEmpty(query))
                {
                    groups = groups.Where(g => g.Name.ToLower().Contains(query.ToLower()));
                }

                // Преобразуем результат в список
                var result = groups.ToList();

                // Возвращаем результат в формате JSON, оборачивая в объект с ключом "groups"
                return Json(new { groups = result });
            }
            catch (Exception ex)
            {
                // Если произошла ошибка (например, проблемы с базой данных), логируем её
                Console.WriteLine($"Ошибка в SearchLessonGroups: {ex.Message}");
                // Возвращаем ошибку в формате JSON с кодом 500
                return StatusCode(500, new { error = "Произошла ошибка на сервере" });
            }
        }

        [HttpGet("/catalog/filter-by-group")]
        public IActionResult FilterLessonsByGroup(string groupName)
        {
            try
            {
                // Получаем все уроки из базы данных, подгружая связанные данные (Level и Group)
                var lessonsQuery = _context.Lessons
                    .Include(l => l.Level)
                    .Include(l => l.Group)
                    .AsQueryable();

                // Если пользователь выбрал группу, фильтруем уроки только по этой группе
                if (!string.IsNullOrEmpty(groupName))
                {
                    lessonsQuery = lessonsQuery.Where(l => l.Group.Name == groupName);
                }

                // Преобразуем результат в список
                var lessons = lessonsQuery.ToList();

                // Группируем уроки по уровням (LevelId) и группам (GroupId), берём только первые 2 урока в каждой группе
                var lessonsByLevelAndGroup = lessons
                    .GroupBy(l => l.LevelId)
                    .ToDictionary(
                        lg => lg.Key,
                        lg => lg
                            .GroupBy(l => l.GroupId)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Take(INITIAL_LESSONS_PER_GROUP).ToList()
                            )
                    );

                // Получаем все уровни и группы для отображения на клиенте
                var levels = _context.Levels.ToList();
                var lessonGroups = _context.LessonGroups.ToList();

                // Формируем объект для ответа, содержащий уровни, группы и сгруппированные уроки
                var response = new
                {
                    levels = levels,
                    lessonGroups = lessonGroups,
                    lessonsByLevelAndGroup = lessonsByLevelAndGroup
                };

                // Возвращаем результат в формате JSON
                return Json(response);
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, логируем её
                Console.WriteLine($"Ошибка в FilterLessonsByGroup: {ex.Message}");
                // Возвращаем ошибку в формате JSON с кодом 500
                return StatusCode(500, new { error = "Произошла ошибка на сервере" });
            }
        }

        [HttpGet("/catalog/load-more")]
        public IActionResult LoadMoreLessons(long levelId, long groupId, int offset, int limit = 2)
        {
            try
            {
                // Получаем уроки для заданного уровня и группы, начиная с offset и ограничивая количеством limit
                var lessons = _context.Lessons
                    .Include(l => l.Level)
                    .Include(l => l.Group)
                    .Where(l => l.LevelId == levelId && l.GroupId == groupId)
                    .OrderBy(l => l.Id) // Упорядочиваем, чтобы подгрузка была последовательной
                    .Skip(offset)
                    .Take(limit)
                    .ToList();

                // Если уроки не найдены, возвращаем JSON с флагом hasMore = false
                if (!lessons.Any())
                {
                    return Json(new { lessons = new List<Lesson>(), hasMore = false });
                }

                // Проверяем, есть ли ещё уроки после текущей подгрузки
                var hasMore = _context.Lessons
                    .Any(l => l.LevelId == levelId && l.GroupId == groupId && l.Id > lessons.Last().Id);

                // Возвращаем уроки и флаг hasMore в формате JSON
                return Json(new { lessons = lessons, hasMore = hasMore });
            }
            catch (Exception ex)
            {
                // Логируем ошибку для отладки
                Console.WriteLine($"Ошибка в LoadMoreLessons: {ex.Message}");
                // Возвращаем ошибку в формате JSON с кодом 500
                return StatusCode(500, new { error = "Произошла ошибка на сервере" });
            }
        }

        [HttpGet("/add-lesson")]
        public IActionResult ShowAddLessonForm()
        {
            var levels = _context.Levels.ToList();
            var lessonGroups = _context.LessonGroups.ToList();

            ViewData["levels"] = levels;
            ViewData["lessonGroups"] = lessonGroups;
            ViewData["lesson"] = new Lesson(); // Пустой объект для формы

            return View("AddLesson");
        }

        [HttpPost("/add-lesson")]
        public IActionResult AddLesson([FromForm] Lesson lesson)
        {
            Console.WriteLine($"Title: {lesson.Title}, FilePath: {lesson.FilePath}, ReleaseDate: {lesson.ReleaseDate}, LevelId: {lesson.LevelId}, GroupId: {lesson.GroupId}");
            try
            {
                // Проверяем существование уровня и группы
                var level = _context.Levels.Find(lesson.LevelId)
                    ?? throw new ArgumentException("Уровень не найден");
                var group = _context.LessonGroups.Find(lesson.GroupId)
                    ?? throw new ArgumentException("Группа уроков не найдена");

                // Серверная валидация filePath
                if (!System.Text.RegularExpressions.Regex.IsMatch(lesson.FilePath, @"^/[a-zA-Z0-9-_/]+\.html$"))
                {
                    throw new ArgumentException("Ссылка на файл должна быть в формате HTML (например, /lesson.html).");
                }

                // Устанавливаем Kind=Utc для ReleaseDate
                lesson.ReleaseDate = DateTime.SpecifyKind(lesson.ReleaseDate, DateTimeKind.Utc);

                _context.Lessons.Add(lesson);
                _context.SaveChanges();

                TempData["success"] = "Урок успешно добавлен!";
                return RedirectToAction("ShowCatalog");
            }
            catch (Exception e)
            {
                TempData["error"] = $"Ошибка при добавлении урока: {e.Message}";
                return RedirectToAction("ShowAddLessonForm");
            }
        }


    }
}