using MediatR;
using MediSync.Application.DTOs.Reminders;

namespace MediSync.Application.Features.Reports.Queries;

public record GetAdherenceStatsQuery(
    Guid   UserId,
    string Period = "month"  // "day" | "week" | "month"
) : IRequest<AdherenceDto>;