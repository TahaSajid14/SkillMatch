using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Data;

namespace SkillMatch.API.Controllers;

[ApiController]
[Route("api/system")]
public class SystemController(SkillMatchDbContext dbContext) : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType<SystemStatusResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SystemStatusResponse>> GetStatus(CancellationToken cancellationToken)
    {
        var databaseAvailable = await dbContext.Database.CanConnectAsync(cancellationToken);

        return Ok(new SystemStatusResponse(
            "SkillMatch API is running.",
            databaseAvailable,
            DateTimeOffset.UtcNow));
    }
}

public record SystemStatusResponse(
    string Message,
    bool DatabaseAvailable,
    DateTimeOffset CheckedAt);
