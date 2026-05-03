using MediatR;
using MediSync.Application.DTOs.Reports;

namespace MediSync.Application.Features.Reports.Queries;

public record GetMonthlyReportQuery(
    Guid UserId,
    int  Month,  // 1-12
    int  Year    // مثال: 2026
) : IRequest<MonthlyReportDto>;