using MediatR;
using MediSync.Application.DTOs.Reminders;
namespace MediSync.Application.Features.Reminders.Queries;
public record GetWeeklyScheduleQuery(Guid UserId)
    : IRequest<List<ReminderDto>>;