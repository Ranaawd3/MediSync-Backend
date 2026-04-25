using MediatR;
namespace MediSync.Application.Features.Reminders.Commands;
public record SkipReminderCommand(Guid ReminderId, Guid UserId) : IRequest;