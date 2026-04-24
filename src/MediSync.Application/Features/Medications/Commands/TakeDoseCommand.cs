using MediatR;

namespace MediSync.Application.Features.Medications.Commands;

public record TakeDoseCommand(Guid MedicationId, Guid UserId) : IRequest;