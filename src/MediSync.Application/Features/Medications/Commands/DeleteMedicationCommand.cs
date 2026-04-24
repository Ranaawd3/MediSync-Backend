using MediatR;

namespace MediSync.Application.Features.Medications.Commands;

public record DeleteMedicationCommand(Guid MedicationId, Guid UserId) : IRequest;