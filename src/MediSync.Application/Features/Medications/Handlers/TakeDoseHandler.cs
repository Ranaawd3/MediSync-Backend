using MediatR;
using MediSync.Application.Features.Medications.Commands;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Medications.Handlers;

public class TakeDoseHandler(IApplicationDbContext db)
    : IRequestHandler<TakeDoseCommand>
{
    public async Task Handle(TakeDoseCommand req, CancellationToken ct)
    {
        // 1. أوجد reminder اليوم للدواء ده
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var reminder = await db.Reminders
            .FirstOrDefaultAsync(r => r.MedicationId == req.MedicationId
                                   && r.UserId == req.UserId
                                   && r.ScheduledDate == today
                                   && r.Status == "pending", ct);

        if (reminder != null)
        {
            reminder.Status = "taken";
            reminder.TakenAt = DateTime.UtcNow;
        }

        // 2. اخصم من الـ StockCount
        var medication = await db.Medications.FindAsync(req.MedicationId, ct);
        if (medication?.StockCount != null && medication.StockCount > 0)
            medication.StockCount--;

        await db.SaveChangesAsync(ct);
    }
}