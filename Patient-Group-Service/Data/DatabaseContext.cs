using Microsoft.EntityFrameworkCore;
using Patient_Group_Service.Models;
using Patient_Group_Service.Models.LinkTables;

namespace Patient_Group_Service.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Caregiver> Caregivers { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<PatientGroup> PatientGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientGroupPatient>().HasKey(t => new { t.PatientId, t.PatientGroupId });
        modelBuilder.Entity<PatientGroupPatient>().HasOne(t => t.Patient).WithMany(t => t.PatientGroupPatients)
            .HasForeignKey(t => t.PatientId);
        modelBuilder.Entity<PatientGroupPatient>().HasOne(t => t.PatientGroup).WithMany(t => t.PatientGroupPatients)
            .HasForeignKey(t => t.PatientGroupId);

        modelBuilder.Entity<PatientGroupCaregiver>().HasKey(t => new { t.CaregiverId, t.PatientGroupId });
        modelBuilder.Entity<PatientGroupCaregiver>().HasOne(t => t.Caregiver).WithMany(t => t.PatientGroupCaregivers)
            .HasForeignKey(t => t.CaregiverId);
        modelBuilder.Entity<PatientGroupCaregiver>().HasOne(t => t.PatientGroup).WithMany(t => t.PatientGroupCaregivers)
            .HasForeignKey(t => t.PatientGroupId);

        modelBuilder.Entity<Caregiver>().ToTable("Caregiver");
        modelBuilder.Entity<Patient>().ToTable("Patient");
        modelBuilder.Entity<PatientGroup>().ToTable("PatientGroup");
    }

}