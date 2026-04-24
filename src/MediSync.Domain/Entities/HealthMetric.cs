namespace MediSync.Domain.Entities;

public class HealthMetric
{
    public Guid      Id             { get; set; } = Guid.NewGuid();
    public Guid      UserId         { get; set; }
    public string    MetricType     { get; set; } = "bp"; // bp | sugar | weight
    public decimal   Value          { get; set; }
    public decimal?  SecondaryValue { get; set; } // للضغط: diastolic
    public string    Unit           { get; set; } = "mmHg";
    public string?   Notes          { get; set; }
    public DateTime  RecordedAt     { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}