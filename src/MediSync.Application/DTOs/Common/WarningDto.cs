namespace MediSync.Application.DTOs.Common;

public record WarningDto(
    string Code,
    string Severity,
    string Message
);