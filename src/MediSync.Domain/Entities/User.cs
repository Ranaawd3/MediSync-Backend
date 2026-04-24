namespace MediSync.Domain.Entities;

public class User
{
    public Guid        Id                  { get; set; } = Guid.NewGuid();
    public string      Email               { get; set; } = "";
    public string      PasswordHash        { get; set; } = "";
    public string      FullName            { get; set; } = "";
    public int         Age                 { get; set; }
    public decimal?    WeightKg            { get; set; }
    public string      Gender              { get; set; } = "other";
    public List<string> ChronicConditions   { get; set; } = [];
    public List<string> Allergies           { get; set; } = [];
    public string      Role                { get; set; } = "patient";
    public bool        IsActive            { get; set; } = true;
    public string?     RefreshToken        { get; set; }
    public DateTime?   RefreshTokenExpiry  { get; set; }
    public string?     PushToken           { get; set; }
    public bool        NotificationEnabled { get; set; } = true;
    public DateTime    CreatedAt           { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Medication> Medications { get; set; } = [];
    public ICollection<Reminder>   Reminders   { get; set; } = [];
}