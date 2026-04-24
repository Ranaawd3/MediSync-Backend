namespace MediSync.Application.DTOs.Medications;

public record UpdateMedicationDto(
    string?  BrandName,
    string?  GenericName,
    decimal? DosageValue,
    string?  DosageUnit,
    int?     TimesPerDay,
    string?  ScheduleType,
    int?     DurationDays,
    int?     StockCount,
    string?  ColorCode
);