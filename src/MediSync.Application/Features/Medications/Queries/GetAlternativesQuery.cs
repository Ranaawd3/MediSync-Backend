using MediatR;

namespace MediSync.Application.Features.Medications.Queries;

public record GetAlternativesQuery(Guid MedicationId, Guid UserId)
    : IRequest<List<string>>;