using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lesson_groups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "levels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LevelId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lessons_lesson_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "lesson_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lessons_levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "lesson_groups",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1L, "Начало работы" },
                    { 2L, "Типы данных" },
                    { 3L, "Коллекции" },
                    { 4L, "Обработка ошибок" },
                    { 5L, "Работа с файлами" },
                    { 6L, "Основы многозадачности" },
                    { 7L, "Сетевое программирование" },
                    { 8L, "Spring Framework" }
                });

            migrationBuilder.InsertData(
                table: "levels",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1L, "Начальный уровень" },
                    { 2L, "Средний уровень" },
                    { 3L, "Продвинутый уровень" }
                });

            migrationBuilder.InsertData(
                table: "lessons",
                columns: new[] { "Id", "FilePath", "GroupId", "LevelId", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1L, "/development-environment.html", 1L, 1L, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 1: Выбор среды разработки" },
                    { 2L, "/syntax.html", 1L, 1L, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 2: Основы синтаксиса" },
                    { 3L, "/OOP.html", 1L, 1L, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 3: Основы ООП" },
                    { 4L, "/primitive-types.html", 2L, 1L, new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 1: Примитивные типы" },
                    { 5L, "/reference-types.html", 2L, 1L, new DateTime(2025, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 2: Ссылочные типы" },
                    { 6L, "/list.html", 3L, 1L, new DateTime(2025, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 1: Списки (List)" },
                    { 7L, "/set.html", 3L, 1L, new DateTime(2025, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 2: Множества (Set)" },
                    { 8L, "/queue.html", 3L, 1L, new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 3: Очереди (Queue)" },
                    { 9L, "/map.html", 3L, 1L, new DateTime(2025, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 4: Карты (Map)" },
                    { 10L, "/exception.html", 4L, 2L, new DateTime(2025, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 1: Исключения (try, catch, finally)" },
                    { 11L, "/create-exception.html", 4L, 2L, new DateTime(2025, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 2: Создание собственных исключений" },
                    { 12L, "/workwithfiles.html", 5L, 2L, new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 1: Чтение и запись файлов" },
                    { 13L, "/threads.html", 6L, 2L, new DateTime(2025, 2, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 1: Потоки" },
                    { 14L, "/synthreads.html", 6L, 2L, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 2: Синхронизация потоков" },
                    { 15L, "/runnable.html", 6L, 2L, new DateTime(2025, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 3: Основы использования Runnable и Thread" },
                    { 16L, "/sockets.html", 7L, 3L, new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 1: Работа с сокетами (TCP/UDP)" },
                    { 17L, "/servers.html", 7L, 3L, new DateTime(2025, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 2: Создание серверов и клиентов" },
                    { 18L, "/httpquery.html", 7L, 3L, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 3: Работа с HTTP-запросами" },
                    { 19L, "/restful.html", 8L, 3L, new DateTime(2025, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Урок 1: RESTful API" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_lessons_GroupId",
                table: "lessons",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_LevelId",
                table: "lessons",
                column: "LevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lessons");

            migrationBuilder.DropTable(
                name: "lesson_groups");

            migrationBuilder.DropTable(
                name: "levels");
        }
    }
}
