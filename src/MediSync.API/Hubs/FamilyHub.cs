using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediSync.API.Hubs;

[Authorize]
public class FamilyHub(IApplicationDbContext db) : Hub
{
    // الـ Caregiver بيـ Subscribe على تحديثات مريض معين
    public async Task SubscribeToPatient(string patientId)
    {
        var caregiverId = Guid.Parse(
            Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // تأكد إن في ربط نشط
        var hasAccess = await db.FamilyLinks.AnyAsync(
            f => f.PatientId   == Guid.Parse(patientId)
              && f.CaregiverId == caregiverId
              && f.Status      == "active");

        if (!hasAccess)
        {
            throw new HubException("لا تملك صلاحية متابعة هذا المريض");
        }

        // انضم لـ Group خاص بالمريض ده
        await Groups.AddToGroupAsync(Context.ConnectionId, $"patient-{patientId}");
    }

    // اتركت الـ Group
    public async Task UnsubscribeFromPatient(string patientId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"patient-{patientId}");
    }
}
