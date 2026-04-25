using MediatR;
using MediSync.Application.Features.Reminders.Commands;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Reminders.Handlers;

public class SnoozeReminderHandler(IApplicationDbContext db)
    : IRequestHandler<SnoozeReminderCommand>
{
    public async Task Handle(SnoozeReminderCommand req, CancellationToken ct)
    {
        var reminder = await db.Reminders
            .FirstOrDefaultAsync(r => r.Id == req.ReminderId
                                   && r.UserId == req.UserId, ct)
            ?? throw new KeyNotFoundException("REMINDER_NOT_FOUND");

        reminder.Status      = "snoozed";
        reminder.SnoozeCount += 1;
        // تحريك الـ ScheduledTime للأمام
        reminder.ScheduledTime = reminder.ScheduledTime.Add(
            TimeSpan.FromMinutes(req.MinutesToSnooze));

        await db.SaveChangesAsync(ct);
    }
}