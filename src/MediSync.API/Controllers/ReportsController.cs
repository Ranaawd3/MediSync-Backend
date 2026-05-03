using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediSync.Application.DTOs.Common;
using MediSync.Application.Features.Reports.Queries;
using MediSync.Infrastructure.Services;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize]
public class ReportsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>التقرير الشهري — بيانات + PDF اختياري</summary>
    [HttpGet("monthly")]
    public async Task<IActionResult> Monthly(
        [FromQuery] int month = 0,
        [FromQuery] int year  = 0,
        [FromQuery] bool pdf  = false)
    {
        var now = DateTime.UtcNow;
        if (month == 0) month = now.Month;
        if (year  == 0) year  = now.Year;

        var result = await mediator.Send(
            new GetMonthlyReportQuery(GetUserId(), month, year));

        // لو طلب PDF — بيرجع PDF File
        if (pdf)
        {
            var pdfBytes = PdfReportGenerator.Generate(result);
            return File(pdfBytes, "application/pdf",
                $"MediSync_Report_{year}_{month:D2}.pdf");
        }

        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>إحصائيات الالتزام التفصيلية</summary>
    [HttpGet("adherence")]
    public async Task<IActionResult> Adherence(
        [FromQuery] string period = "month")
    {
        var result = await mediator.Send(
            new GetAdherenceStatsQuery(GetUserId(), period));
        return Ok(ApiResponse<object>.Ok(result));
    }
}