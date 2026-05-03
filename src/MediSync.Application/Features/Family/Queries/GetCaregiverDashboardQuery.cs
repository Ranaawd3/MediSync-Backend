using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Family;

namespace MediSync.Application.Features.Family.Queries;

public record GetCaregiverDashboardQuery(
    Guid PatientId,
    Guid CaregiverId   // للتحقق من الصلاحية
) : IRequest<ApiResponse<CaregiverDashboardDto>>;