namespace MediSync.Domain.Entities;

public class SymptomReport
{
    public Guid          Id           { get; set; } = Guid.NewGuid();
    public Guid          UserId       { get; set; }
    public Guid?         MedicationId { get; set; }
    public List<string>  Symptoms     { get; set; } = [];
    public string        Severity     { get; set; } = "LOW";
    public DateTime      ReportedAt   { get; set; } = DateTime.UtcNow;

    public User       User       { get; set; } = null!;
    public Medication? Medication { get; set; }
}