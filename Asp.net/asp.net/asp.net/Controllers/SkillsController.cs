using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Data;
using SkillMatch.API.DTOs.Skills;
using SkillMatch.API.Interfaces;

namespace SkillMatch.API.Controllers;

[ApiController]
[Authorize]
[Route("api/skills")]
public sealed class SkillsController(
    SkillMatchDbContext dbContext,
    ISkillExtractionService skillExtractionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SkillResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SkillResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var skills = await dbContext.Skills
            .AsNoTracking()
            .OrderBy(skill => skill.Category)
            .ThenBy(skill => skill.Name)
            .Select(skill => new SkillResponse(skill.Id, skill.Name, skill.Category))
            .ToListAsync(cancellationToken);

        return Ok(skills);
    }

    [HttpPost("extract")]
    [ProducesResponseType<SkillExtractionResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SkillExtractionResponse>> Extract(
        ExtractSkillsRequest request,
        CancellationToken cancellationToken)
    {
        var skills = await skillExtractionService.ExtractSkillsAsync(request.Text, cancellationToken);
        var response = skills
            .Select(skill => new SkillResponse(skill.Id, skill.Name, skill.Category))
            .OrderBy(skill => skill.Category)
            .ThenBy(skill => skill.Name)
            .ToList();

        return Ok(new SkillExtractionResponse(response.Count, response));
    }
}
