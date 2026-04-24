using System.Security.Claims;
using MediSync.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/sync")]
[Authorize]
public class SyncController(AppDbContext db) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>Flutter بتبعت الجرعات اللي اتأكدت offline</summary>
    [HttpPost("doses")]
    public async Task<IActionResult> SyncOfflineDoses(
        [FromBody] List<OfflineDoseDto> doses)
    {
        var userId = GetUserId();
        int synced = 0;

        foreach (var dose in doses)
        {
            var scheduledDate = DateOnly.FromDateTime(dose.TakenAt.ToUniversalTime());

            var reminder = await db.Reminders
                .FirstOrDefaultAsync(r =>
                    r.MedicationId    == dose.MedicationId &&
                    r.UserId          == userId &&
                    r.ScheduledDate   == scheduledDate &&
                    r.Status          == "pending");

            if (reminder != null)
            {
                reminder.Status  = dose.Status;
                reminder.TakenAt = dose.TakenAt.ToUniversalTime();

                // اخصم من المخزون لو اتأخد
                if (dose.Status == "taken")
                {
                    var med = await db.Medications.FindAsync(dose.MedicationId);
                    if (med?.StockCount != null && med.StockCount > 0)
                        med.StockCount--;
                }

                synced++;
            }
        }

        await db.SaveChangesAsync();
        return Ok(new { success = true, synced, total = doses.Count });
    }
}

public record OfflineDoseDto(
    Guid     MedicationId,
    DateTime TakenAt,
    string   Status   // taken | skipped
);