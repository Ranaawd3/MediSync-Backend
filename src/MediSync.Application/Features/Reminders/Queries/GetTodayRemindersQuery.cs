using MediatR;
using MediSync.Application.DTOs.Reminders;
namespace MediSync.Application.Features.Reminders.Queries;
public record GetTodayRemindersQuery(Guid UserId)
    : IRequest<List<ReminderDto>>;