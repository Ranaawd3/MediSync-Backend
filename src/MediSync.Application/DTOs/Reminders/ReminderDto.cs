namespace MediSync.Application.DTOs.Reminders;

// متوافق مع Contract Section 7 — Reminder Variables
public record ReminderDto(
    Guid    Id,
    Guid    MedicationId,
    string  MedicationName,
    Guid    UserId,
    string  ScheduledDate,     // YYYY-MM-DD
    string  ScheduledTime,     // HH:mm:ss
    string? TakenAt,           // ISO 8601
    string  Status,            // pending | taken | missed | snoozed | skipped
    int     SnoozeCount,
    bool    CaregiverNotified
);

public record AdherenceDto(
    Guid    UserId,
    string  Period,              // day | week | month
    int     TotalDoses,
    int     TakenDoses,
    int     MissedDoses,
    decimal AdherencePercentage, // 0-100
    int     CurrentStreak,
    int     LongestStreak
);