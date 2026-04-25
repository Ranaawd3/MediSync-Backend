using MediatR;
using MediSync.Application.DTOs.Reminders;
using MediSync.Application.Features.Reminders.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Reminders.Handlers;

public class GetWeeklyScheduleHandler(IApplicationDbContext db)
    : IRequestHandler<GetWeeklyScheduleQuery, List<ReminderDto>>
{
    public async Task<List<ReminderDto>> Handle(
        GetWeeklyScheduleQuery req, CancellationToken ct)
    {
        var today   = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekEnd = today.AddDays(7);

        return await db.Reminders
            .Include(r => r.Medication)
            .Where(r => r.UserId == req.UserId
                     && r.ScheduledDate >= today
                     && r.ScheduledDate <= weekEnd)
            .OrderBy(r => r.ScheduledDate).ThenBy(r => r.ScheduledTime)
            .Select(r => new ReminderDto(
                r.Id, r.MedicationId, r.Medication.BrandName, r.UserId,
                r.ScheduledDate.ToString("yyyy-MM-dd"),
                r.ScheduledTime.ToString("HH:mm:ss"),
                r.TakenAt == null ? null : r.TakenAt.Value.ToString("o"),
                r.Status, r.SnoozeCount, r.CaregiverNotified))
            .ToListAsync(ct);
    }
}