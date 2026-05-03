namespace MediSync.Application.DTOs.HealthMetrics;

public record AddHealthMetricDto(
    string   MetricType,      // "bp" | "sugar" | "weight"
    decimal  Value,
    decimal? SecondaryValue,
    string   Unit,
    string?  Notes
);