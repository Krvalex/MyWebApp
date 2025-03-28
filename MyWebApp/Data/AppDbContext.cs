using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonGroup> LessonGroups { get; set; }
        public DbSet<Level> Levels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Укажи, что Id является первичным ключом и автоинкрементом
            modelBuilder.Entity<Level>().Property(l => l.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<LessonGroup>().Property(lg => lg.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Lesson>().Property(l => l.Id).ValueGeneratedOnAdd();

            // Начальные данные для Levels
            modelBuilder.Entity<Level>().HasData(
                new Level { Id = 1, Name = "Начальный уровень" },
                new Level { Id = 2, Name = "Средний уровень" },
                new Level { Id = 3, Name = "Продвинутый уровень" }
            );

            // Начальные данные для LessonGroups
            modelBuilder.Entity<LessonGroup>().HasData(
                new LessonGroup { Id = 1, Name = "Начало работы" },
                new LessonGroup { Id = 2, Name = "Типы данных" },
                new LessonGroup { Id = 3, Name = "Коллекции" },
                new LessonGroup { Id = 4, Name = "Обработка ошибок" },
                new LessonGroup { Id = 5, Name = "Работа с файлами" },
                new LessonGroup { Id = 6, Name = "Основы многозадачности" },
                new LessonGroup { Id = 7, Name = "Сетевое программирование" },
                new LessonGroup { Id = 8, Name = "Spring Framework" }
            );

            // Начальные данные для Lessons
            modelBuilder.Entity<Lesson>().HasData(
                // Уроки для уровня "beginner"
                new Lesson { Id = 1, Title = "Урок 1: Выбор среды разработки", FilePath = "/development-environment.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-03-12"), DateTimeKind.Utc), LevelId = 1, GroupId = 1 },
                new Lesson { Id = 2, Title = "Урок 2: Основы синтаксиса", FilePath = "/syntax.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-5"), DateTimeKind.Utc), LevelId = 1, GroupId = 1 },
                new Lesson { Id = 3, Title = "Урок 3: Основы ООП", FilePath = "/OOP.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-3"), DateTimeKind.Utc), LevelId = 1, GroupId = 1 },
                new Lesson { Id = 4, Title = "Урок 1: Примитивные типы", FilePath = "/primitive-types.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-27"), DateTimeKind.Utc), LevelId = 1, GroupId = 2 },
                new Lesson { Id = 5, Title = "Урок 2: Ссылочные типы", FilePath = "/reference-types.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-11"), DateTimeKind.Utc), LevelId = 1, GroupId = 2 },
                new Lesson { Id = 6, Title = "Урок 1: Списки (List)", FilePath = "/list.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-03-9"), DateTimeKind.Utc), LevelId = 1, GroupId = 3 },
                new Lesson { Id = 7, Title = "Урок 2: Множества (Set)", FilePath = "/set.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-25"), DateTimeKind.Utc), LevelId = 1, GroupId = 3 },
                new Lesson { Id = 8, Title = "Урок 3: Очереди (Queue)", FilePath = "/queue.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-14"), DateTimeKind.Utc), LevelId = 1, GroupId = 3 },
                new Lesson { Id = 9, Title = "Урок 4: Карты (Map)", FilePath = "/map.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-03-7"), DateTimeKind.Utc), LevelId = 1, GroupId = 3 },

                // Уроки для уровня "intermediate"
                new Lesson { Id = 10, Title = "Урок 1: Исключения (try, catch, finally)", FilePath = "/exception.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-23"), DateTimeKind.Utc), LevelId = 2, GroupId = 4 },
                new Lesson { Id = 11, Title = "Урок 2: Создание собственных исключений", FilePath = "/create-exception.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-17"), DateTimeKind.Utc), LevelId = 2, GroupId = 4 },
                new Lesson { Id = 12, Title = "Урок 1: Чтение и запись файлов", FilePath = "/workwithfiles.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-03-12"), DateTimeKind.Utc), LevelId = 2, GroupId = 5 },
                new Lesson { Id = 13, Title = "Урок 1: Потоки", FilePath = "/threads.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-21"), DateTimeKind.Utc), LevelId = 2, GroupId = 6 },
                new Lesson { Id = 14, Title = "Урок 2: Синхронизация потоков", FilePath = "/synthreads.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-01"), DateTimeKind.Utc), LevelId = 2, GroupId = 6 },
                new Lesson { Id = 15, Title = "Урок 3: Основы использования Runnable и Thread", FilePath = "/runnable.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-02"), DateTimeKind.Utc), LevelId = 2, GroupId = 6 },

                // Уроки для уровня "advanced"
                new Lesson { Id = 16, Title = "Урок 1: Работа с сокетами (TCP/UDP)", FilePath = "/sockets.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-03"), DateTimeKind.Utc), LevelId = 3, GroupId = 7 },
                new Lesson { Id = 17, Title = "Урок 2: Создание серверов и клиентов", FilePath = "/servers.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-03-11"), DateTimeKind.Utc), LevelId = 3, GroupId = 7 },
                new Lesson { Id = 18, Title = "Урок 3: Работа с HTTP-запросами", FilePath = "/httpquery.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-02-05"), DateTimeKind.Utc), LevelId = 3, GroupId = 7 },
                new Lesson { Id = 19, Title = "Урок 1: RESTful API", FilePath = "/restful.html", ReleaseDate = DateTime.SpecifyKind(DateTime.Parse("2025-03-13"), DateTimeKind.Utc), LevelId = 3, GroupId = 8 }
            );
        }
    }
}