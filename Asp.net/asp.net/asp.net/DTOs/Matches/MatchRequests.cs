using System.ComponentModel.DataAnnotations;

namespace SkillMatch.API.DTOs.Matches;

public sealed class AnalyzeMatchRequest
{
    [Range(1, int.MaxValue)]
    public int ResumeId { get; init; }

    [Range(1, int.MaxValue)]
    public int JobId { get; init; }
}
