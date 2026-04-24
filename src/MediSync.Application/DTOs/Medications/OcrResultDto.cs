namespace MediSync.Application.DTOs.Medications;

// متوافق مع Contract Section 7 — OCR Variables
public record OcrResultDto(
    Guid                        ScanId,
    decimal                     Confidence,
    string                      Status,           // success | low_confidence | failed
    bool                        RequiresManualReview,
    string?                     RawText,
    List<MedicationExtractedDto> ExtractedMedications,
    int                         ProcessingTimeMs
);

public record MedicationExtractedDto(
    string  BrandName,
    string  GenericName,
    string  ActiveIngredient,
    decimal DosageValue,
    string  DosageUnit,
    int     TimesPerDay,
    string  ScheduleType,
    int?    DurationDays,
    string? Instructions,
    ConfidencePerFieldDto ConfidencePerField
);

public record ConfidencePerFieldDto(decimal BrandName, decimal Dosage, decimal TimesPerDay);