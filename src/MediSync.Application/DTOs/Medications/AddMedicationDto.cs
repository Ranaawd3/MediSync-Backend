namespace MediSync.Application.DTOs.Medications;

public record AddMedicationDto(
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
    int?     StockCount,
    string   ColorCode,
    string   Source
);