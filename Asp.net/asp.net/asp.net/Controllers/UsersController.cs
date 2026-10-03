using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Data;
using SkillMatch.API.DTOs.Auth;

namespace SkillMatch.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(SkillMatchDbContext dbContext) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType<AuthenticatedUser>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticatedUser>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .Where(candidate => candidate.Id == userId)
            .Select(candidate => new AuthenticatedUser(
                candidate.Id,
                candidate.FullName,
                candidate.Email,
                candidate.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken);

        return user is null ? Unauthorized() : Ok(user);
    }
}
