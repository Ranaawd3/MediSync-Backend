using MediatR;
using MediSync.Application.DTOs.Medications;

namespace MediSync.Application.Features.Medications.Commands;

public record UpdateMedicationCommand(Guid MedicationId, Guid UserId, UpdateMedicationDto Dto)
    : IRequest<MedicationDto>;