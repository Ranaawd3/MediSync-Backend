namespace MediSync.Application.DTOs.HealthMetrics;

// متوافق مع Contract Section 7 — Health Metrics Variables
public record HealthMetricDto(
    Guid     Id,
    Guid     UserId,
    string   MetricType,      // "bp" | "sugar" | "weight"
    decimal  Value,
    decimal? SecondaryValue,  // للضغط: diastolic
    string   Unit,            // "mmHg" | "mg/dL" | "kg"
    string?  Notes,
    string   RecordedAt       // ISO 8601
);