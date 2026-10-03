using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Data;
using SkillMatch.API.DTOs.Matches;
using SkillMatch.API.Helpers;
using SkillMatch.API.Interfaces;
using SkillMatch.API.Models;

namespace SkillMatch.API.Services;

public sealed class JobMatchingService(SkillMatchDbContext dbContext) : IJobMatchingService
{
    public async Task<AnalyzeMatchResult> AnalyzeAsync(
        int userId,
        AnalyzeMatchRequest request,
        CancellationToken cancellationToken)
    {
        var resume = await dbContext.Resumes
            .AsNoTracking()
            .Include(candidate => candidate.ResumeSkills)
                .ThenInclude(resumeSkill => resumeSkill.Skill)
            .SingleOrDefaultAsync(
                candidate => candidate.Id == request.ResumeId && candidate.UserId == userId,
                cancellationToken);
        if (resume is null)
        {
            return AnalyzeMatchResult.Failure("The selected resume was not found.");
        }

        var job = await dbContext.Jobs
            .AsNoTracking()
            .Include(candidate => candidate.JobSkills)
                .ThenInclude(jobSkill => jobSkill.Skill)
            .SingleOrDefaultAsync(
                candidate => candidate.Id == request.JobId && candidate.UserId == userId,
                cancellationToken);
        if (job is null)
        {
            return AnalyzeMatchResult.Failure("The selected job was not found.");
        }

        var resumeSkillNames = resume.ResumeSkills
            .Select(resumeSkill => resumeSkill.Skill.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var requiredSkills = job.JobSkills
            .Where(jobSkill => jobSkill.Importance == SkillImportance.Required)
            .Select(jobSkill => jobSkill.Skill)
            .OrderBy(skill => skill.Name)
            .ToList();
        var matchedSkills = requiredSkills
            .Where(skill => SkillCompatibility.IsRequirementCovered(skill.Name, resumeSkillNames))
            .ToList();
        var missingSkills = requiredSkills
            .Where(skill => !SkillCompatibility.IsRequirementCovered(skill.Name, resumeSkillNames))
            .ToList();
        var score = requiredSkills.Count == 0
            ? 0m
            : Math.Round(
                (decimal)matchedSkills.Count / requiredSkills.Count * 100m,
                2,
                MidpointRounding.AwayFromZero);

        var match = new JobMatch
        {
            UserId = userId,
            ResumeId = resume.Id,
            JobId = job.Id,
            MatchScore = score,
            MatchedSkillsCount = matchedSkills.Count,
            MissingSkillsCount = missingSkills.Count
        };

        foreach (var skill in requiredSkills)
        {
            match.MatchSkills.Add(new JobMatchSkill
            {
                JobMatch = match,
                SkillId = skill.Id,
                IsMatched = SkillCompatibility.IsRequirementCovered(skill.Name, resumeSkillNames)
            });
        }

        dbContext.JobMatches.Add(match);
        await dbContext.SaveChangesAsync(cancellationToken);

        return AnalyzeMatchResult.Success(new MatchResultResponse(
            match.Id,
            resume.Id,
            resume.FileName,
            job.Id,
            job.Title,
            job.Company,
            score,
            matchedSkills.Count,
            missingSkills.Count,
            matchedSkills.Select(ToSkillResponse).ToList(),
            missingSkills.Select(ToSkillResponse).ToList(),
            match.CreatedAt));
    }

    public async Task<IReadOnlyList<MatchHistoryResponse>> GetHistoryAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.JobMatches
            .AsNoTracking()
            .Where(match => match.UserId == userId)
            .OrderByDescending(match => match.CreatedAt)
            .Select(match => new MatchHistoryResponse(
                match.Id,
                match.ResumeId,
                match.Resume.FileName,
                match.JobId,
                match.Job.Title,
                match.Job.Company,
                match.MatchScore,
                match.MatchedSkillsCount,
                match.MissingSkillsCount,
                match.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<MatchResultResponse?> GetByIdAsync(
        int userId,
        int matchId,
        CancellationToken cancellationToken)
    {
        var match = await dbContext.JobMatches
            .AsNoTracking()
            .Include(candidate => candidate.Resume)
            .Include(candidate => candidate.Job)
            .Include(candidate => candidate.MatchSkills)
                .ThenInclude(matchSkill => matchSkill.Skill)
            .SingleOrDefaultAsync(
                candidate => candidate.Id == matchId && candidate.UserId == userId,
                cancellationToken);

        if (match is null)
        {
            return null;
        }

        var matchedSkills = match.MatchSkills
            .Where(matchSkill => matchSkill.IsMatched)
            .Select(matchSkill => matchSkill.Skill)
            .OrderBy(skill => skill.Name)
            .Select(ToSkillResponse)
            .ToList();
        var missingSkills = match.MatchSkills
            .Where(matchSkill => !matchSkill.IsMatched)
            .Select(matchSkill => matchSkill.Skill)
            .OrderBy(skill => skill.Name)
            .Select(ToSkillResponse)
            .ToList();

        return new MatchResultResponse(
            match.Id,
            match.ResumeId,
            match.Resume.FileName,
            match.JobId,
            match.Job.Title,
            match.Job.Company,
            match.MatchScore,
            match.MatchedSkillsCount,
            match.MissingSkillsCount,
            matchedSkills,
            missingSkills,
            match.CreatedAt);
    }

    private static MatchSkillResponse ToSkillResponse(Skill skill) =>
        new(skill.Id, skill.Name, skill.Category);
}
