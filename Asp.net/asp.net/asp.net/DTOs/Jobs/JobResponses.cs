namespace SkillMatch.API.DTOs.Jobs;

public sealed record JobSummaryResponse(
    int Id,
    string Title,
    string Company,
    DateTimeOffset CreatedAt,
    int DetectedSkillCount);

public sealed record JobDetailResponse(
    int Id,
    string Title,
    string Company,
    string Description,
    DateTimeOffset CreatedAt,
    IReadOnlyList<JobSkillResponse> DetectedSkills);

public sealed record JobSkillResponse(
    int Id,
    string Name,
    string Category,
    string Importance);
