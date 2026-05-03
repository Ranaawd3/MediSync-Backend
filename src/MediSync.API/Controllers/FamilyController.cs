using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Family;
using MediSync.Application.Features.Family.Commands;
using MediSync.Application.Features.Family.Queries;
using System.Security.Claims;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/family")]
[Authorize]
public class FamilyController(IMediator mediator) : ControllerBase
{
    // Helper — جيبي الـ UserId من الـ JWT
    private Guid UserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>إرسال دعوة لـ Caregiver</summary>
    [HttpPost("invite")]
    public async Task<IActionResult> Invite([FromBody] InviteCaregiverDto dto)
    {
        var result = await mediator.Send(new InviteCaregiverCommand(UserId(), dto));
        return StatusCode(201, result);
    }

    /// <summary>قبول دعوة المقرب عبر التوكن</summary>
    [HttpPost("accept/{token}")]
    public async Task<IActionResult> Accept(string token)
    {
        var result = await mediator.Send(new AcceptInviteCommand(UserId(), token));
        return Ok(result);
    }

    /// <summary>لوحة تحكم المقرب للمريض المحدد</summary>
    [HttpGet("{patientId}/dashboard")]
    public async Task<IActionResult> GetDashboard(Guid patientId)
    {
        var result = await mediator.Send(
            new GetCaregiverDashboardQuery(patientId, UserId()));
        return Ok(result);
    }

    /// <summary>إلغاء صلاحية المقرب</summary>
    [HttpDelete("{linkId}")]
    public async Task<IActionResult> Revoke(Guid linkId)
    {
        var result = await mediator.Send(new RevokeFamilyLinkCommand(linkId, UserId()));
        return Ok(result);
    }
}