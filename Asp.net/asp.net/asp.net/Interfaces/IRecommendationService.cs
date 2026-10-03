using SkillMatch.API.DTOs.Recommendations;

namespace SkillMatch.API.Interfaces;

public interface IRecommendationService
{
    Task<RecommendationPlanResponse?> GetForMatchAsync(
        int userId,
        int matchId,
        CancellationToken cancellationToken);
}
