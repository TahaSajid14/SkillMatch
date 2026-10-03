using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Data;
using SkillMatch.API.DTOs.Jobs;
using SkillMatch.API.Interfaces;
using SkillMatch.API.Models;

namespace SkillMatch.API.Services;

public sealed class JobService(
    SkillMatchDbContext dbContext,
    ISkillExtractionService skillExtractionService) : IJobService
{
    public async Task<IReadOnlyList<JobSummaryResponse>> GetAllAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Jobs
            .AsNoTracking()
            .Where(job => job.UserId == userId)
            .OrderByDescending(job => job.CreatedAt)
            .Select(job => new JobSummaryResponse(
                job.Id,
                job.Title,
                job.Company,
                job.CreatedAt,
                job.JobSkills.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<JobDetailResponse?> GetByIdAsync(
        int userId,
        int jobId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Jobs
            .AsNoTracking()
            .Where(job => job.Id == jobId && job.UserId == userId)
            .Select(job => new JobDetailResponse(
                job.Id,
                job.Title,
                job.Company,
                job.Description,
                job.CreatedAt,
                job.JobSkills
                    .OrderBy(jobSkill => jobSkill.Skill.Name)
                    .Select(jobSkill => new JobSkillResponse(
                        jobSkill.Skill.Id,
                        jobSkill.Skill.Name,
                        jobSkill.Skill.Category,
                        jobSkill.Importance.ToString()))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<JobDetailResponse> CreateAsync(
        int userId,
        JobUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var detectedSkills = await skillExtractionService.ExtractSkillsAsync(
            request.Description,
            cancellationToken);
        var job = new Job
        {
            UserId = userId,
            Title = request.Title.Trim(),
            Company = request.Company.Trim(),
            Description = request.Description.Trim()
        };

        foreach (var skill in detectedSkills)
        {
            job.JobSkills.Add(new JobSkill
            {
                Job = job,
                Skill = skill,
                SkillId = skill.Id,
                Importance = SkillImportance.Required
            });
        }

        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDetail(job, detectedSkills);
    }

    public async Task<JobDetailResponse?> UpdateAsync(
        int userId,
        int jobId,
        JobUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs
            .Include(candidate => candidate.JobSkills)
            .SingleOrDefaultAsync(
                candidate => candidate.Id == jobId && candidate.UserId == userId,
                cancellationToken);

        if (job is null)
        {
            return null;
        }

        var detectedSkills = await skillExtractionService.ExtractSkillsAsync(
            request.Description,
            cancellationToken);
        var detectedSkillIds = detectedSkills.Select(skill => skill.Id).ToHashSet();
        var existingSkillIds = job.JobSkills.Select(jobSkill => jobSkill.SkillId).ToHashSet();

        dbContext.JobSkills.RemoveRange(
            job.JobSkills.Where(jobSkill => !detectedSkillIds.Contains(jobSkill.SkillId)));

        foreach (var jobSkill in job.JobSkills.Where(item => detectedSkillIds.Contains(item.SkillId)))
        {
            jobSkill.Importance = SkillImportance.Required;
        }

        foreach (var skill in detectedSkills.Where(skill => !existingSkillIds.Contains(skill.Id)))
        {
            job.JobSkills.Add(new JobSkill
            {
                JobId = job.Id,
                SkillId = skill.Id,
                Job = job,
                Skill = skill,
                Importance = SkillImportance.Required
            });
        }

        job.Title = request.Title.Trim();
        job.Company = request.Company.Trim();
        job.Description = request.Description.Trim();
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDetail(job, detectedSkills);
    }

    public async Task<DeleteJobResult> DeleteAsync(
        int userId,
        int jobId,
        CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs
            .SingleOrDefaultAsync(
                candidate => candidate.Id == jobId && candidate.UserId == userId,
                cancellationToken);

        if (job is null)
        {
            return DeleteJobResult.NotFound;
        }

        var hasMatchHistory = await dbContext.JobMatches
            .AnyAsync(jobMatch => jobMatch.JobId == jobId, cancellationToken);
        if (hasMatchHistory)
        {
            return DeleteJobResult.HasMatchHistory;
        }

        dbContext.Jobs.Remove(job);
        await dbContext.SaveChangesAsync(cancellationToken);
        return DeleteJobResult.Deleted;
    }

    private static JobDetailResponse ToDetail(Job job, IReadOnlyList<Skill> skills)
    {
        var responseSkills = skills
            .Select(skill => new JobSkillResponse(
                skill.Id,
                skill.Name,
                skill.Category,
                SkillImportance.Required.ToString()))
            .OrderBy(skill => skill.Name)
            .ToList();

        return new JobDetailResponse(
            job.Id,
            job.Title,
            job.Company,
            job.Description,
            job.CreatedAt,
            responseSkills);
    }
}
