using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.HealthMetrics;
using MediSync.Application.Features.HealthMetrics.Commands;
using MediSync.Application.Features.HealthMetrics.Queries;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/health-metrics")]
[Authorize]
public class HealthMetricsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>تسجيل قراءة صحية جديدة</summary>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddHealthMetricDto dto)
    {
        var result = await mediator.Send(
            new AddHealthMetricCommand(dto, GetUserId()));
        return StatusCode(201, ApiResponse<HealthMetricDto>.Ok(result, "تم تسجيل القراءة"));
    }

    /// <summary>تاريخ القراءات الصحية</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? metricType = null)
    {
        var result = await mediator.Send(
            new GetHealthMetricsQuery(GetUserId(), metricType));
        return Ok(ApiResponse<List<HealthMetricDto>>.Ok(result));
    }
}