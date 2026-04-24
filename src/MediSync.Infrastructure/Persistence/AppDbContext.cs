using Microsoft.EntityFrameworkCore;
using MediSync.Domain.Entities;
using MediSync.Application.Persistence;

namespace MediSync.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IApplicationDbContext  // ← أضيفي الـ Interface
{
    public DbSet<User>            Users            => Set<User>();
    public DbSet<Medication>      Medications      => Set<Medication>();
    public DbSet<Reminder>        Reminders        => Set<Reminder>();
    public DbSet<DrugInteraction>  DrugInteractions => Set<DrugInteraction>();
    public DbSet<DrugNameMapping>  DrugNameMappings => Set<DrugNameMapping>();
    public DbSet<FamilyLink>       FamilyLinks      => Set<FamilyLink>();
    public DbSet<HealthMetric>     HealthMetrics    => Set<HealthMetric>();
    public DbSet<OcrScan>          OcrScans         => Set<OcrScan>();
    public DbSet<NotificationLog>  NotificationLogs => Set<NotificationLog>();
    public DbSet<AuditLog>         AuditLogs        => Set<AuditLog>();
    public DbSet<SymptomReport> SymptomReports => Set<SymptomReport>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // User
        b.Entity<User>(e => {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.ChronicConditions).HasColumnType("text[]");
            e.Property(u => u.Allergies).HasColumnType("text[]");
        });

        // DrugInteraction — unique pair
        b.Entity<DrugInteraction>(e => {
            e.HasIndex(d => new { d.Drug1Ingredient, d.Drug2Ingredient }).IsUnique();
            e.Property(d => d.Alternatives).HasColumnType("text[]");
        });

        // Medication → User
        b.Entity<Medication>(e => {
            e.HasOne(m => m.User)
             .WithMany(u => u.Medications)
             .HasForeignKey(m => m.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Reminder → Medication + User
        b.Entity<Reminder>(e => {
            e.HasOne(r => r.Medication)
             .WithMany(m => m.Reminders)
             .HasForeignKey(r => r.MedicationId);
            e.HasOne(r => r.User)
             .WithMany(u => u.Reminders)
             .HasForeignKey(r => r.UserId);
        });
    }
}