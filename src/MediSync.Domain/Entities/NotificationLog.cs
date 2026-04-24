namespace MediSync.Domain.Entities;

public class NotificationLog
{
    public Guid      Id        { get; set; } = Guid.NewGuid();
    public Guid      UserId    { get; set; }
    public string    Type      { get; set; } = "";
    public string    Title     { get; set; } = "";
    public string    Body      { get; set; } = "";
    public string    Channel   { get; set; } = "push";
    public DateTime  SentAt    { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt    { get; set; }

    public User User { get; set; } = null!;
}