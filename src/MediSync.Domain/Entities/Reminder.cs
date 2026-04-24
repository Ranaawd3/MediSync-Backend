namespace MediSync.Domain.Entities;

public class Reminder
{
    public Guid      Id                { get; set; } = Guid.NewGuid();
    public Guid      MedicationId      { get; set; }
    public Guid      UserId            { get; set; }
    public DateOnly  ScheduledDate     { get; set; }
    public TimeOnly  ScheduledTime     { get; set; }
    public DateTime? TakenAt           { get; set; }
    public string    Status            { get; set; } = "pending";
    public int       SnoozeCount       { get; set; } = 0;
    public bool      CaregiverNotified { get; set; } = false;
    public DateTime  CreatedAt         { get; set; } = DateTime.UtcNow;

    // Navigation
    public Medication Medication { get; set; } = null!;
    public User       User       { get; set; } = null!;
}