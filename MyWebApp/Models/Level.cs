using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyWebApp.Models // Замени MyWebApp на имя твоего проекта
{
    [Table("levels")]
    public class Level
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")] // Укажи тип данных, если нужно
        public string Name { get; set; }

        [JsonIgnore] // Игнорируем обратную ссылку на уроки при сериализации
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}