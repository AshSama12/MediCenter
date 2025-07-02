using Microsoft.EntityFrameworkCore;
using mediCenter.Models;

namespace mediCenter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(p => p.PatientId);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Drug)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DrugId);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId);

            // Configure decimal precision for drug prices
            modelBuilder.Entity<Drug>()
                .Property(d => d.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}