using MediatR;
using MediSync.Application.DTOs.HealthMetrics;

namespace MediSync.Application.Features.HealthMetrics.Queries;

public record GetHealthMetricsQuery(
    Guid    UserId,
    string? MetricType = null  // null = كل الأنواع
) : IRequest<List<HealthMetricDto>>;