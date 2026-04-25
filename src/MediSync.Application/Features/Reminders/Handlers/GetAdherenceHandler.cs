using MediatR;
using MediSync.Application.DTOs.Reminders;
using MediSync.Application.Features.Reminders.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Reminders.Handlers;

public class GetAdherenceHandler(IApplicationDbContext db)
    : IRequestHandler<GetAdherenceQuery, AdherenceDto>
{
    public async Task<AdherenceDto> Handle(
        GetAdherenceQuery req, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly from = req.Period switch
        {
            "day"   => today,
            "month" => today.AddDays(-30),
            _        => today.AddDays(-7)   // week default
        };

        var reminders = await db.Reminders
            .Where(r => r.UserId == req.UserId
                     && r.ScheduledDate >= from
                     && r.ScheduledDate <= today)
            .ToListAsync(ct);

        var total  = reminders.Count;
        var taken  = reminders.Count(r => r.Status == "taken");
        var missed = reminders.Count(r => r.Status == "missed");
        var pct    = total > 0 ? Math.Round((decimal)taken / total * 100, 1) : 0;

        // حساب الـ Streak
        var currentStreak = 0;
        var checkDate     = today;
        while (true)
        {
            var dayReminders = reminders.Where(r => r.ScheduledDate == checkDate).ToList();
            if (!dayReminders.Any() || dayReminders.Any(r => r.Status == "missed"))
                break;
            currentStreak++;
            checkDate = checkDate.AddDays(-1);
        }

        return new AdherenceDto(
            req.UserId, req.Period, total, taken, missed, pct, currentStreak, currentStreak);
    }
}