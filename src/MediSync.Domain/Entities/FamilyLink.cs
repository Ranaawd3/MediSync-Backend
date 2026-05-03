namespace MediSync.Domain.Entities;

public class FamilyLink
{
    public Guid     Id            { get; set; } = Guid.NewGuid();

    // الـ Patient — صاحب الأدوية
    public Guid     PatientId     { get; set; }
    public User     Patient       { get; set; } = null!;

    // الـ Caregiver — المقرّب اللي بيراقب
    public Guid?    CaregiverId   { get; set; }     // null قبل القبول
    public User?    Caregiver     { get; set; }

    // بيانات الدعوة
    public string   CaregiverEmail { get; set; } = string.Empty;
    public string   InviteToken   { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime InviteExpiry  { get; set; } = DateTime.UtcNow.AddDays(7);

    // حالة الربط
    public string   Status        { get; set; } = "pending"; // pending | active | revoked
    public bool     CanEditMeds   { get; set; } = false;      // صلاحية التعديل
    public bool     ReceiveAlerts { get; set; } = true;       // استلام إشعارات

    public DateTime CreatedAt     { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt   { get; set; }
}