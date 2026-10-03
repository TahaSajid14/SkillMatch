using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Data;
using SkillMatch.API.DTOs.Dashboard;
using SkillMatch.API.Interfaces;

namespace SkillMatch.API.Services;

public sealed class DashboardService(SkillMatchDbContext dbContext) : IDashboardService
{
    public async Task<DashboardResponse> GetAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var totalResumes = await dbContext.Resumes
            .CountAsync(resume => resume.UserId == userId, cancellationToken);
        var totalJobs = await dbContext.Jobs
            .CountAsync(job => job.UserId == userId, cancellationToken);
        var totalJobsAnalyzed = await dbContext.JobMatches
            .Where(match => match.UserId == userId)
            .Select(match => match.JobId)
            .Distinct()
            .CountAsync(cancellationToken);

        var scores = await dbContext.JobMatches
            .AsNoTracking()
            .Where(match => match.UserId == userId)
            .Select(match => match.MatchScore)
            .ToListAsync(cancellationToken);

        var missingSkillRows = await dbContext.JobMatchSkills
            .AsNoTracking()
            .Where(matchSkill => matchSkill.JobMatch.UserId == userId && !matchSkill.IsMatched)
            .Select(matchSkill => new
            {
                matchSkill.SkillId,
                matchSkill.Skill.Name,
                matchSkill.Skill.Category
            })
            .ToListAsync(cancellationToken);

        var mostCommonMissingSkills = missingSkillRows
            .GroupBy(matchSkill => new
            {
                matchSkill.SkillId,
                matchSkill.Name,
                matchSkill.Category
            })
            .Select(group => new MissingSkillStatResponse(
                group.Key.SkillId,
                group.Key.Name,
                group.Key.Category,
                group.Count()))
            .OrderByDescending(skill => skill.MissingCount)
            .ThenBy(skill => skill.Name)
            .Take(5)
            .ToList();

        var distinctResumeSkills = await dbContext.ResumeSkills
            .AsNoTracking()
            .Where(resumeSkill => resumeSkill.Resume.UserId == userId)
            .Select(resumeSkill => new
            {
                resumeSkill.SkillId,
                resumeSkill.Skill.Category
            })
            .Distinct()
            .ToListAsync(cancellationToken);

        var uniqueSkillCount = distinctResumeSkills.Count;
        var skillDistribution = distinctResumeSkills
            .GroupBy(skill => skill.Category)
            .Select(group => new SkillCategoryStatResponse(
                group.Key,
                group.Count(),
                uniqueSkillCount == 0
                    ? 0m
                    : Math.Round(
                        (decimal)group.Count() / uniqueSkillCount * 100m,
                        1,
                        MidpointRounding.AwayFromZero)))
            .OrderByDescending(category => category.SkillCount)
            .ThenBy(category => category.Category)
            .ToList();

        var recentAnalyses = await dbContext.JobMatches
            .AsNoTracking()
            .Where(match => match.UserId == userId)
            .OrderByDescending(match => match.CreatedAt)
            .Take(5)
            .Select(match => new RecentAnalysisResponse(
                match.Id,
                match.Job.Title,
                match.Job.Company,
                match.Resume.FileName,
                match.MatchScore,
                match.MatchedSkillsCount,
                match.MissingSkillsCount,
                match.CreatedAt))
            .ToListAsync(cancellationToken);

        return new DashboardResponse(
            totalResumes,
            totalJobs,
            totalJobsAnalyzed,
            scores.Count,
            scores.Count == 0
                ? 0m
                : Math.Round(scores.Average(), 2, MidpointRounding.AwayFromZero),
            scores.Count == 0 ? 0m : scores.Max(),
            mostCommonMissingSkills,
            skillDistribution,
            recentAnalyses);
    }
}
