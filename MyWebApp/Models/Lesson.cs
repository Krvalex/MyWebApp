using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models // Замени MyWebApp на имя твоего проекта
{
    [Table("lessons")]
    public class Lesson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [ForeignKey("Level")]
        public long LevelId { get; set; }
        public virtual Level Level { get; set; }

        [ForeignKey("Group")]
        public long GroupId { get; set; }
        public virtual LessonGroup Group { get; set; }
    }
}