namespace MediSync.Domain.Entities;

public class AuditLog
{
    public Guid     Id          { get; set; } = Guid.NewGuid();
    public Guid?    UserId      { get; set; }
    public string   Action      { get; set; } = "";
    public string   EntityType  { get; set; } = "";
    public string?  EntityId    { get; set; }
    public string?  OldValue    { get; set; }
    public string?  NewValue    { get; set; }
    public string?  IpAddress   { get; set; }
    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;
}