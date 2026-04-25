using MediatR;
using MediSync.Application.Features.Reminders.Commands;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Reminders.Handlers;

public class SkipReminderHandler(IApplicationDbContext db)
    : IRequestHandler<SkipReminderCommand>
{
    public async Task Handle(SkipReminderCommand req, CancellationToken ct)
    {
        var reminder = await db.Reminders
            .FirstOrDefaultAsync(r => r.Id == req.ReminderId
                                   && r.UserId == req.UserId, ct)
            ?? throw new KeyNotFoundException("REMINDER_NOT_FOUND");

        reminder.Status = "skipped";
        await db.SaveChangesAsync(ct);
    }
}