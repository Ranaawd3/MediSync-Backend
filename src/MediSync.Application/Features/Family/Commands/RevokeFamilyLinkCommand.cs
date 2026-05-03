using MediatR;
using MediSync.Application.DTOs.Common;

namespace MediSync.Application.Features.Family.Commands;

public record RevokeFamilyLinkCommand(
    Guid LinkId,
    Guid RequesterId   // الـ Patient اللي طالب الإلغاء
) : IRequest<ApiResponse<object>>;