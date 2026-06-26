using System;
using System.ComponentModel.DataAnnotations;

namespace ElMaherQuranSchool.Models
{
    public enum RegistrationStatus { Pending, Accepted, Rejected }

    public class RegistrationRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StudentName { get; set; } = string.Empty;

        public int? Age { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        [Required]
        [StringLength(100)]
        public string ParentName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ParentPhone { get; set; } = string.Empty;

        [StringLength(200)]
        public string LastLevelOfMemorization { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        public string? ProfileImageUrl { get; set; }

        public int? PreferredHalaqaId { get; set; }

        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Halaqa? PreferredHalaqa { get; set; }
    }
}