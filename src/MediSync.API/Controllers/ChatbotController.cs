using System.Security.Claims;
using MediSync.Application.DTOs.Common;
using MediSync.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/chatbot")]
[Authorize]
public class ChatbotController(IChatbotService chatbot) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>سؤال للمساعد الصيدلاني</summary>
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Question))
            return BadRequest(new { success = false, 
                error = new { code = "VALIDATION_ERROR", message = "السؤال فاضي" }});

        var answer = await chatbot.AskAsync(req.Question, GetUserId());

        return Ok(ApiResponse<object>.Ok(new { answer }));
    }
}

public record ChatRequest(string Question);