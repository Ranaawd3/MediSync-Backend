namespace MediSync.Application.Services;

public interface IEmailService
{
    Task SendCaregiverInviteAsync(
        string caregiverEmail,
        string patientName,
        string acceptUrl);

    Task SendMissedDoseAlertAsync(
        string caregiverEmail,
        string patientName,
        string medicationName,
        string scheduledTime);
}