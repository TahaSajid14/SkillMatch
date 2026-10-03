namespace SkillMatch.API.DTOs.Recommendations;

public sealed record RecommendationPlanResponse(
    int MatchId,
    int MissingSkillsCount,
    IReadOnlyList<LearningRecommendationResponse> Recommendations);

public sealed record LearningRecommendationResponse(
    int Id,
    int SkillId,
    string SkillName,
    string Category,
    string Title,
    string Description,
    string LearningPriority);
