using MediatR;
using MediSync.Application.DTOs.Interactions;

namespace MediSync.Application.Features.Interactions.Queries;

public record CheckInteractionsQuery(Guid UserId)
    : IRequest<List<InteractionResultDto>>;