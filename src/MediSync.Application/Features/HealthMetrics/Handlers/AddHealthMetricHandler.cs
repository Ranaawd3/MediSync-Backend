using MediatR;
using MediSync.Application.DTOs.HealthMetrics;
using MediSync.Application.Features.HealthMetrics.Commands;
using MediSync.Application.Persistence;
using MediSync.Domain.Entities;

namespace MediSync.Application.Features.HealthMetrics.Handlers;

public class AddHealthMetricHandler(IApplicationDbContext db)
    : IRequestHandler<AddHealthMetricCommand, HealthMetricDto>
{
    public async Task<HealthMetricDto> Handle(
        AddHealthMetricCommand req, CancellationToken ct)
    {
        // التحقق من الـ MetricType صح
        if (req.Dto.MetricType is not ("bp" or "sugar" or "weight"))
            throw new InvalidOperationException("VALIDATION_ERROR");

        var metric = new HealthMetric
        {
            UserId         = req.UserId,
            MetricType     = req.Dto.MetricType,
            Value          = req.Dto.Value,
            SecondaryValue = req.Dto.SecondaryValue,
            Unit           = req.Dto.Unit,
            Notes          = req.Dto.Notes,
            RecordedAt     = DateTime.UtcNow,
        };

        db.HealthMetrics.Add(metric);
        await db.SaveChangesAsync(ct);

        return new HealthMetricDto(
            metric.Id, metric.UserId, metric.MetricType,
            metric.Value, metric.SecondaryValue, metric.Unit,
            metric.Notes, metric.RecordedAt.ToString("o"));
    }
}