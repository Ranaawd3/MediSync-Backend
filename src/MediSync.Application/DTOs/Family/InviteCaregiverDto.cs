namespace MediSync.Application.DTOs.Family;

public record InviteCaregiverDto(
    string CaregiverEmail,
    bool   CanEditMeds   = false,
    bool   ReceiveAlerts = true
);