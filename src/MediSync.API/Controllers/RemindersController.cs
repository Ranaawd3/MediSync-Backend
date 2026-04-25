using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Reminders;
using MediSync.Application.Features.Reminders.Commands;
using MediSync.Application.Features.Reminders.Queries;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/reminders")]
[Authorize]
public class RemindersController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>تذكيرات اليوم</summary>
    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var result = await mediator.Send(new GetTodayRemindersQuery(GetUserId()));
        return Ok(ApiResponse<List<ReminderDto>>.Ok(result));
    }

    /// <summary>الجدول الأسبوعي</summary>
    [HttpGet("schedule")]
    public async Task<IActionResult> GetSchedule()
    {
        var result = await mediator.Send(new GetWeeklyScheduleQuery(GetUserId()));
        return Ok(ApiResponse<List<ReminderDto>>.Ok(result));
    }

    /// <summary>تأجيل التذكير</summary>
    [HttpPut("{id}/snooze")]
    public async Task<IActionResult> Snooze(Guid id, [FromQuery] int minutes = 15)
    {
        await mediator.Send(new SnoozeReminderCommand(id, GetUserId(), minutes));
        return Ok(ApiResponse<object>.Ok(new {}, "تم تأجيل التذكير"));
    }

    /// <summary>تخطي الجرعة</summary>
    [HttpPut("{id}/skip")]
    public async Task<IActionResult> Skip(Guid id)
    {
        await mediator.Send(new SkipReminderCommand(id, GetUserId()));
        return Ok(ApiResponse<object>.Ok(new {}, "تم تخطي الجرعة"));
    }

    /// <summary>نسبة الالتزام</summary>
    [HttpGet("adherence")]
    public async Task<IActionResult> GetAdherence([FromQuery] string period = "week")
    {
        var result = await mediator.Send(new GetAdherenceQuery(GetUserId(), period));
        return Ok(ApiResponse<AdherenceDto>.Ok(result));
    }
}