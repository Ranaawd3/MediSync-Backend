using MediatR;
namespace MediSync.Application.Features.Reminders.Commands;
public record SnoozeReminderCommand(Guid ReminderId, Guid UserId, int MinutesToSnooze = 15)
    : IRequest;