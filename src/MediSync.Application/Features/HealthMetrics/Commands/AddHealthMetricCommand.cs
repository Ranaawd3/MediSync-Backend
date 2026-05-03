using MediatR;
using MediSync.Application.DTOs.HealthMetrics;

namespace MediSync.Application.Features.HealthMetrics.Commands;

public record AddHealthMetricCommand(AddHealthMetricDto Dto, Guid UserId)
    : IRequest<HealthMetricDto>;