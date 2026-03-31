// src/FeatureFlagService.API/Controllers/FlagsController.cs
using FeatureFlagService.Application.Commands.DeleteFlag;
using FeatureFlagService.Application.Commands.UpsertFlag;
using FeatureFlagService.Application.Queries.EvaluateFlag;
using FeatureFlagService.Application.Queries.GetAllFlags;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagService.API.Controllers;

[ApiController]
[Route("api/flags")]
public class FlagsController : ControllerBase
{
    private readonly IMediator _mediator;
    public FlagsController(IMediator mediator) => _mediator = mediator;

    // GET /api/flags
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _mediator.Send(new GetAllFlagsQuery()));

    // POST /api/flags
    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertFlagCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return Ok(new { id });
    }

    // GET /api/flags/{key}/evaluate?userId=abc&role=admin
    [HttpGet("{key}/evaluate")]
    public async Task<IActionResult> Evaluate(
        string key,
        [FromQuery] string userId,
        [FromQuery] string role = "")
    {
        var enabled = await _mediator.Send(new EvaluateFlagQuery(key, userId, role));
        return Ok(new { key, userId, enabled });
    }

    // DELETE /api/flags/{key}
    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(string key)
    {
        await _mediator.Send(new DeleteFlagCommand(key));
        return NoContent();
    }
}
