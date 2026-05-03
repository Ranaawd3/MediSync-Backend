namespace MediSync.Application.Services;

public interface IFamilyHubContext
{
    Task NotifyDoseStatusChangedAsync(
        Guid patientId,
        Guid reminderId,
        string newStatus,
        string medicationName);
}