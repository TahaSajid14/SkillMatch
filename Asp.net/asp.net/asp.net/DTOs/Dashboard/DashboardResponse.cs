namespace SkillMatch.API.DTOs.Dashboard;

public sealed record DashboardResponse(
    int TotalResumes,
    int TotalJobs,
    int TotalJobsAnalyzed,
    int TotalAnalyses,
    decimal AverageMatchScore,
    decimal HighestMatchScore,
    IReadOnlyList<MissingSkillStatResponse> MostCommonMissingSkills,
    IReadOnlyList<SkillCategoryStatResponse> SkillDistribution,
    IReadOnlyList<RecentAnalysisResponse> RecentAnalyses);

public sealed record MissingSkillStatResponse(
    int SkillId,
    string Name,
    string Category,
    int MissingCount);

public sealed record SkillCategoryStatResponse(
    string Category,
    int SkillCount,
    decimal Percentage);

public sealed record RecentAnalysisResponse(
    int Id,
    string JobTitle,
    string Company,
    string ResumeFileName,
    decimal MatchScore,
    int MatchedSkillsCount,
    int MissingSkillsCount,
    DateTimeOffset CreatedAt);
