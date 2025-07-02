using System.ComponentModel.DataAnnotations;

namespace mediCenter.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        // Explicitly marked as optional with nullable reference types
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property for prescriptions
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        // Computed property for age
        public int Age => DateTime.Now.Year - BirthDate.Year - (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? 1 : 0);

        // Full name property
        public string FullName => $"{FirstName} {LastName}";
    }
}