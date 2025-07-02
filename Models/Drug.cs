using System.ComponentModel.DataAnnotations;

namespace mediCenter.Models
{
    public class Drug
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; } // Optional

        [Required]
        public string Manufacturer { get; set; }

        public string? Category { get; set; } // Optional

        [Required]
        [Display(Name = "Unit Price (LKR)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Display(Name = "Units per Pack")]
        public int UnitsPerPack { get; set; } = 1; // Default to 1

        [Display(Name = "Pack Type")]
        public string PackType { get; set; } = "Unit"; // Unit, Pack, Bottle, etc.

        public int StockQuantity { get; set; }

        public string? Dosage { get; set; } // Optional

        public string? SideEffects { get; set; } // Optional

        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        // Computed property for total price per pack
        public decimal PackPrice => UnitPrice * UnitsPerPack;

        // Navigation property
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
