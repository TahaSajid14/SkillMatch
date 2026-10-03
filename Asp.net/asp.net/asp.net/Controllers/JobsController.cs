using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMatch.API.DTOs.Jobs;
using SkillMatch.API.Interfaces;

namespace SkillMatch.API.Controllers;

[ApiController]
[Authorize]
[Route("api/jobs")]
public sealed class JobsController(IJobService jobService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<JobSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<JobSummaryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await jobService.GetAllAsync(GetUserId(), cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<JobDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var job = await jobService.GetByIdAsync(GetUserId(), id, cancellationToken);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost]
    [ProducesResponseType<JobDetailResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<JobDetailResponse>> Create(
        JobUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var job = await jobService.CreateAsync(GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<JobDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailResponse>> Update(
        int id,
        JobUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var job = await jobService.UpdateAsync(GetUserId(), id, request, cancellationToken);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await jobService.DeleteAsync(GetUserId(), id, cancellationToken) switch
        {
            DeleteJobResult.Deleted => NoContent(),
            DeleteJobResult.NotFound => NotFound(),
            DeleteJobResult.HasMatchHistory => Conflict(new ProblemDetails
            {
                Title = "Job cannot be deleted",
                Detail = "This job has match history and must be retained.",
                Status = StatusCodes.Status409Conflict
            }),
            _ => throw new InvalidOperationException("Unknown job deletion result.")
        };
    }

    private int GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("The user identifier is missing.");
    }
}
