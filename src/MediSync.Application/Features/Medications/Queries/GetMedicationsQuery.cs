using MediatR;
using MediSync.Application.DTOs.Medications;

namespace MediSync.Application.Features.Medications.Queries;

public record GetMedicationsQuery(Guid UserId)
    : IRequest<List<MedicationDto>>;