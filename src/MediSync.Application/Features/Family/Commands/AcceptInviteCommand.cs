using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Family;

namespace MediSync.Application.Features.Family.Commands;

public record AcceptInviteCommand(
    Guid   CaregiverId,
    string Token
) : IRequest<ApiResponse<FamilyLinkDto>>;