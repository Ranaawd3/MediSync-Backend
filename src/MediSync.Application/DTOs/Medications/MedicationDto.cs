namespace MediSync.Application.DTOs.Medications;

// متوافق مع Contract Section 7 — Medication Variables
public record MedicationDto(
    Guid     Id,
    Guid     UserId,
    string   BrandName,
    string   GenericName,
    string   ActiveIngredient,
    decimal  DosageValue,
    string   DosageUnit,
    string   Form,
    int      TimesPerDay,
    string   ScheduleType,
    int?     DurationDays,
    string   StartDate,
    string?  EndDate,
    int?     StockCount,
    string   ColorCode,
    string   Source,
    bool     IsActive,
    string   CreatedAt
);
