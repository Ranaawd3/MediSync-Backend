namespace MediSync.Domain.Entities;

public class FamilyLink
{
    public Guid     Id           { get; set; } = Guid.NewGuid();
    public Guid     PatientId    { get; set; }
    public Guid     CaregiverId  { get; set; }
    public string   Status       { get; set; } = "pending"; // pending | accepted | revoked
    public string?  InviteToken  { get; set; }
    public bool     ViewMedications { get; set; } = true;
    public bool     ViewAdherence   { get; set; } = true;
    public bool     ReceiveAlerts   { get; set; } = true;
    public DateTime CreatedAt    { get; set; } = DateTime.UtcNow;

    // Navigation
    public User Patient   { get; set; } = null!;
    public User Caregiver { get; set; } = null!;
}