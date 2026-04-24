using MediatR;
using MediSync.Application.DTOs.Medications;

namespace MediSync.Application.Features.Medications.Queries;

public record GetMedicationByIdQuery(Guid MedicationId, Guid UserId)
    : IRequest<MedicationDto>;