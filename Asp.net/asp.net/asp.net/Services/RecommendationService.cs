using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Data;
using SkillMatch.API.DTOs.Recommendations;
using SkillMatch.API.Interfaces;
using SkillMatch.API.Models;

namespace SkillMatch.API.Services;

public sealed class RecommendationService(SkillMatchDbContext dbContext) : IRecommendationService
{
    public async Task<RecommendationPlanResponse?> GetForMatchAsync(
        int userId,
        int matchId,
        CancellationToken cancellationToken)
    {
        var match = await dbContext.JobMatches
            .AsNoTracking()
            .Where(candidate => candidate.Id == matchId && candidate.UserId == userId)
            .Select(candidate => new
            {
                candidate.Id,
                candidate.MissingSkillsCount
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (match is null)
        {
            return null;
        }

        var recommendations = await dbContext.JobMatchSkills
            .AsNoTracking()
            .Where(matchSkill => matchSkill.JobMatchId == matchId && !matchSkill.IsMatched)
            .SelectMany(matchSkill => matchSkill.Skill.Recommendations)
            .OrderByDescending(recommendation =>
                recommendation.LearningPriority == LearningPriority.High
                    ? 3
                    : recommendation.LearningPriority == LearningPriority.Medium
                        ? 2
                        : 1)
            .ThenBy(recommendation => recommendation.Skill.Name)
            .Select(recommendation => new LearningRecommendationResponse(
                recommendation.Id,
                recommendation.SkillId,
                recommendation.Skill.Name,
                recommendation.Skill.Category,
                recommendation.Title,
                recommendation.Description,
                recommendation.LearningPriority.ToString()))
            .ToListAsync(cancellationToken);

        return new RecommendationPlanResponse(
            match.Id,
            match.MissingSkillsCount,
            recommendations);
    }
}
