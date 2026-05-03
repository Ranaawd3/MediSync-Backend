using MediatR;
using MediSync.Application.DTOs.Reminders;
using MediSync.Application.Features.Reports.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Reports.Handlers;

public class GetAdherenceStatsHandler(IApplicationDbContext db)
    : IRequestHandler<GetAdherenceStatsQuery, AdherenceDto>
{
    public async Task<AdherenceDto> Handle(
        GetAdherenceStatsQuery req, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from  = req.Period switch
        {
            "day"  => today,
            "week" => today.AddDays(-7),
            _       => today.AddDays(-30)  // month default
        };

        var reminders = await db.Reminders
            .Where(r => r.UserId        == req.UserId
                     && r.ScheduledDate >= from
                     && r.ScheduledDate <= today)
            .ToListAsync(ct);

        var total  = reminders.Count;
        var taken  = reminders.Count(r => r.Status == "taken");
        var missed = reminders.Count(r => r.Status == "missed");
        var pct    = total > 0
            ? Math.Round((decimal)taken / total * 100, 1)
            : 0m;

        // حساب الـ Streak
        int streak    = 0;
        var checkDate = today;
        while (true)
        {
            var dayRem = reminders
                .Where(r => r.ScheduledDate == checkDate).ToList();
            if (!dayRem.Any() || dayRem.Any(r => r.Status == "missed"))
                break;
            streak++;
            checkDate = checkDate.AddDays(-1);
        }

        return new AdherenceDto(
            req.UserId, req.Period,
            total, taken, missed, pct,
            streak, streak);
    }
}