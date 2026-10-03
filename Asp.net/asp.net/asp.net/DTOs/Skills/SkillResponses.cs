using System.ComponentModel.DataAnnotations;

namespace SkillMatch.API.DTOs.Skills;

public sealed record SkillResponse(int Id, string Name, string Category);

public sealed class ExtractSkillsRequest
{
    [Required, StringLength(50000, MinimumLength = 1)]
    public string Text { get; init; } = string.Empty;
}

public sealed record SkillExtractionResponse(
    int Count,
    IReadOnlyList<SkillResponse> Skills);
