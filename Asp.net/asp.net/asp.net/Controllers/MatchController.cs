using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMatch.API.DTOs.Matches;
using SkillMatch.API.DTOs.Recommendations;
using SkillMatch.API.Interfaces;

namespace SkillMatch.API.Controllers;

[ApiController]
[Authorize]
[Route("api/match")]
public sealed class MatchController(
    IJobMatchingService matchingService,
    IRecommendationService recommendationService) : ControllerBase
{
    [HttpPost("analyze")]
    [ProducesResponseType<MatchResultResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MatchResultResponse>> Analyze(
        AnalyzeMatchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await matchingService.AnalyzeAsync(GetUserId(), request, cancellationToken);
        if (!result.Succeeded)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Match analysis failed",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Response!.Id }, result.Response);
    }

    [HttpGet("history")]
    [ProducesResponseType<IReadOnlyList<MatchHistoryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MatchHistoryResponse>>> GetHistory(
        CancellationToken cancellationToken)
    {
        return Ok(await matchingService.GetHistoryAsync(GetUserId(), cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<MatchResultResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MatchResultResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var match = await matchingService.GetByIdAsync(GetUserId(), id, cancellationToken);
        return match is null ? NotFound() : Ok(match);
    }

    [HttpGet("{id:int}/recommendations")]
    [ProducesResponseType<RecommendationPlanResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecommendationPlanResponse>> GetRecommendations(
        int id,
        CancellationToken cancellationToken)
    {
        var plan = await recommendationService.GetForMatchAsync(
            GetUserId(),
            id,
            cancellationToken);
        return plan is null ? NotFound() : Ok(plan);
    }

    private int GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("The user identifier is missing.");
    }
}
