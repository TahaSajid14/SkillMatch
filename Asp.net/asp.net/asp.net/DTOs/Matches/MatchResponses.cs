namespace SkillMatch.API.DTOs.Matches;

public sealed record MatchSkillResponse(int Id, string Name, string Category);

public sealed record MatchResultResponse(
    int Id,
    int ResumeId,
    string ResumeFileName,
    int JobId,
    string JobTitle,
    string Company,
    decimal MatchScore,
    int MatchedSkillsCount,
    int MissingSkillsCount,
    IReadOnlyList<MatchSkillResponse> MatchedSkills,
    IReadOnlyList<MatchSkillResponse> MissingSkills,
    DateTimeOffset CreatedAt);

public sealed record MatchHistoryResponse(
    int Id,
    int ResumeId,
    string ResumeFileName,
    int JobId,
    string JobTitle,
    string Company,
    decimal MatchScore,
    int MatchedSkillsCount,
    int MissingSkillsCount,
    DateTimeOffset CreatedAt);
