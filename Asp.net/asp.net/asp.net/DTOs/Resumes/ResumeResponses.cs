namespace SkillMatch.API.DTOs.Resumes;

public sealed record ResumeSummaryResponse(
    int Id,
    string FileName,
    DateTimeOffset UploadedAt,
    int ExtractedCharacterCount,
    int DetectedSkillCount);

public sealed record ResumeDetailResponse(
    int Id,
    string FileName,
    string ExtractedText,
    DateTimeOffset UploadedAt,
    IReadOnlyList<DetectedSkillResponse> DetectedSkills);

public sealed record ResumeUploadResponse(
    int Id,
    string FileName,
    DateTimeOffset UploadedAt,
    int ExtractedCharacterCount,
    IReadOnlyList<DetectedSkillResponse> DetectedSkills,
    string Message);

public sealed record DetectedSkillResponse(int Id, string Name, string Category);
