using MediatR;
using MediSync.Application.DTOs.HealthMetrics;
using MediSync.Application.Features.HealthMetrics.Queries;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.HealthMetrics.Handlers;

public class GetHealthMetricsHandler(IApplicationDbContext db)
    : IRequestHandler<GetHealthMetricsQuery, List<HealthMetricDto>>
{
    public async Task<List<HealthMetricDto>> Handle(
        GetHealthMetricsQuery req, CancellationToken ct)
    {
        var query = db.HealthMetrics
            .Where(h => h.UserId == req.UserId);

        if (req.MetricType != null)
            query = query.Where(h => h.MetricType == req.MetricType);

        return await query
            .OrderByDescending(h => h.RecordedAt)
            .Take(100)  // آخر 100 قراءة
            .Select(h => new HealthMetricDto(
                h.Id, h.UserId, h.MetricType,
                h.Value, h.SecondaryValue, h.Unit,
                h.Notes, h.RecordedAt.ToString("o")))
            .ToListAsync(ct);
    }
}