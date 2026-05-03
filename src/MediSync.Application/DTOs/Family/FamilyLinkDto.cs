namespace MediSync.Application.DTOs.Family;

public record FamilyLinkDto(
    Guid    Id,
    Guid    PatientId,
    string  PatientName,
    Guid?   CaregiverId,
    string  CaregiverEmail,
    string  Status,          // pending | active | revoked
    bool    CanEditMeds,
    bool    ReceiveAlerts,
    DateTime CreatedAt,
    DateTime? AcceptedAt
);

public record CaregiverDashboardDto(
    Guid   PatientId,
    string PatientName,
    int    TodayTotal,       // إجمالي تذكيرات اليوم
    int    TodayTaken,       // تم تأكيدها
    int    TodayMissed,      // فاتت
    int    TodayPending,     // لسه
    double AdherencePercent, // نسبة الالتزام آخر 7 أيام
    IEnumerable<ReminderSummaryDto> TodayReminders
);

public record ReminderSummaryDto(
    Guid   ReminderId,
    string MedicationName,
    string DosageText,       // "500mg"
    string ScheduledTime,    // "08:00"
    string Status            // taken | missed | pending
);