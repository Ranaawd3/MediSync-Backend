using MediSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Persistence;

public interface IApplicationDbContext
{
    DbSet<User>            Users            { get; }
    DbSet<Medication>      Medications      { get; }
    DbSet<Reminder>        Reminders        { get; }
    DbSet<DrugInteraction>  DrugInteractions { get; }
    DbSet<DrugNameMapping>  DrugNameMappings { get; }
    DbSet<FamilyLink>       FamilyLinks      { get; }
    DbSet<HealthMetric>     HealthMetrics    { get; }
    DbSet<OcrScan>          OcrScans         { get; }
    DbSet<NotificationLog>  NotificationLogs { get; }
    DbSet<AuditLog>         AuditLogs        { get; }
    DbSet<SymptomReport>    SymptomReports   { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}