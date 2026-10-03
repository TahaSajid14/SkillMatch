using System.ComponentModel.DataAnnotations;

namespace SkillMatch.API.DTOs.Jobs;

public sealed class JobUpsertRequest
{
    [Required, StringLength(200, MinimumLength = 2)]
    public string Title { get; init; } = string.Empty;

    [Required, StringLength(200, MinimumLength = 2)]
    public string Company { get; init; } = string.Empty;

    [Required, StringLength(50000, MinimumLength = 10)]
    public string Description { get; init; } = string.Empty;
}
