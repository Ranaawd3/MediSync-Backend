using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Family;

namespace MediSync.Application.Features.Family.Commands;

public record InviteCaregiverCommand(
    Guid                 PatientId,
    InviteCaregiverDto   Dto
) : IRequest<ApiResponse<FamilyLinkDto>>;