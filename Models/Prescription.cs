using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mediCenter.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DrugId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Instructions { get; set; }

        public DateTime PrescribedDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Quantity (Packs/Units)")]
        public int Quantity { get; set; }

        public string Notes { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        // Computed property for total amount
        [NotMapped]
        public decimal TotalAmount => Drug?.PackPrice * Quantity ?? 0;

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DrugId")]
        public virtual Drug Drug { get; set; }

        [ForeignKey("DoctorId")]
        public virtual User Doctor { get; set; }
    }
}
