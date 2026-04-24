namespace MediSync.Application.DTOs.Auth;

public record RegisterDto(
    string       Email,
    string       Password,
    string       FullName,
    int          Age,
    decimal?     WeightKg,
    string       Gender,
    List<string> ChronicConditions,
    List<string> Allergies
);