using MediatR;
using MediSync.Application.DTOs.Reminders;
namespace MediSync.Application.Features.Reminders.Queries;
public record GetAdherenceQuery(Guid UserId, string Period = "week")
    : IRequest<AdherenceDto>;