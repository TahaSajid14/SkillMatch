using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMatch.API.DTOs.Resumes;
using SkillMatch.API.Interfaces;

namespace SkillMatch.API.Controllers;

[ApiController]
[Authorize]
[Route("api/resumes")]
public sealed class ResumesController(IResumeService resumeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ResumeSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ResumeSummaryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await resumeService.GetAllAsync(GetUserId(), cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ResumeDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumeDetailResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var resume = await resumeService.GetByIdAsync(GetUserId(), id, cancellationToken);
        return resume is null ? NotFound() : Ok(resume);
    }

    [HttpGet("{id:int}/file")]
    [ProducesResponseType<FileStreamResult>(StatusCodes.Status200OK, "application/pdf")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        int id,
        CancellationToken cancellationToken)
    {
        var file = await resumeService.OpenFileAsync(GetUserId(), id, cancellationToken);
        return file is null
            ? NotFound()
            : File(file.Stream, file.ContentType, file.FileName);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    [ProducesResponseType<ResumeUploadResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResumeUploadResponse>> Upload(
        [FromForm] ResumeUploadRequest request,
        CancellationToken cancellationToken)
    {
        var result = await resumeService.UploadAsync(GetUserId(), request.File, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Resume upload failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Response!.Id }, result.Response);
    }

    [HttpPost("{id:int}/extract-skills")]
    [ProducesResponseType<ResumeDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumeDetailResponse>> ReanalyzeSkills(
        int id,
        CancellationToken cancellationToken)
    {
        var resume = await resumeService.ReanalyzeSkillsAsync(GetUserId(), id, cancellationToken);
        return resume is null ? NotFound() : Ok(resume);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await resumeService.DeleteAsync(GetUserId(), id, cancellationToken) switch
        {
            DeleteResumeResult.Deleted => NoContent(),
            DeleteResumeResult.NotFound => NotFound(),
            DeleteResumeResult.HasMatchHistory => Conflict(new ProblemDetails
            {
                Title = "Resume cannot be deleted",
                Detail = "This resume has match history and must be retained.",
                Status = StatusCodes.Status409Conflict
            }),
            _ => throw new InvalidOperationException("Unknown resume deletion result.")
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
