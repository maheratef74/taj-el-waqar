using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElMaherQuranSchool.Models
{
    public class Halaqa
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(200)]
        public string Schedule { get; set; } = string.Empty;

        public int? TeacherId { get; set; }

        public int TargetPages { get; set; } = 30;

        [StringLength(50)]
        public string? Level { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        public int MaxCapacity { get; set; } = 20;

        [StringLength(100)]
        public string? AgeRange { get; set; }

        [StringLength(50)]
        public string? ClassTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Teacher? Teacher { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
