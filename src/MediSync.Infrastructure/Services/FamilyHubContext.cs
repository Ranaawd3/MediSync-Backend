using Microsoft.AspNetCore.SignalR;
using MediSync.Application.Services;

namespace MediSync.Infrastructure.Services;

public class FamilyHubContext : IFamilyHubContext
{
    private readonly IHubContext<Hub> _hub;

    public FamilyHubContext(IHubContext<Hub> hub)
    {
        _hub = hub;
    }

    public async Task NotifyDoseStatusChangedAsync(
        Guid patientId,
        Guid reminderId,
        string newStatus,
        string medicationName)
    {
        await _hub.Clients
            .Group($"patient-{patientId}")
            .SendAsync("DoseStatusUpdated", new
            {
                reminderId,
                status = newStatus,
                medicationName,
                updatedAt = DateTime.UtcNow
            });
    }
}