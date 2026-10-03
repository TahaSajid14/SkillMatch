using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkillMatch.API.Configuration;
using SkillMatch.API.Data;
using SkillMatch.API.DTOs.Resumes;
using SkillMatch.API.Interfaces;
using SkillMatch.API.Models;

namespace SkillMatch.API.Services;

public sealed class ResumeService(
    SkillMatchDbContext dbContext,
    IResumeStorageService storageService,
    IResumeParserService parserService,
    ISkillExtractionService skillExtractionService,
    IOptions<ResumeStorageOptions> storageOptions,
    ILogger<ResumeService> logger) : IResumeService
{
    private static readonly byte[] PdfSignature = "%PDF-"u8.ToArray();
    private static readonly byte[] ZipSignature = [0x50, 0x4B, 0x03, 0x04];
    private readonly ResumeStorageOptions _storageOptions = storageOptions.Value;

    public async Task<IReadOnlyList<ResumeSummaryResponse>> GetAllAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Resumes
            .AsNoTracking()
            .Where(resume => resume.UserId == userId)
            .OrderByDescending(resume => resume.UploadedAt)
            .Select(resume => new ResumeSummaryResponse(
                resume.Id,
                resume.FileName,
                resume.UploadedAt,
                resume.ExtractedText == null ? 0 : resume.ExtractedText.Length,
                resume.ResumeSkills.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<ResumeDetailResponse?> GetByIdAsync(
        int userId,
        int resumeId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Resumes
            .AsNoTracking()
            .Where(resume => resume.Id == resumeId && resume.UserId == userId)
            .Select(resume => new ResumeDetailResponse(
                resume.Id,
                resume.FileName,
                resume.ExtractedText ?? string.Empty,
                resume.UploadedAt,
                resume.ResumeSkills
                    .OrderBy(resumeSkill => resumeSkill.Skill.Name)
                    .Select(resumeSkill => new DetectedSkillResponse(
                        resumeSkill.Skill.Id,
                        resumeSkill.Skill.Name,
                        resumeSkill.Skill.Category))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ResumeFileResult?> OpenFileAsync(
        int userId,
        int resumeId,
        CancellationToken cancellationToken)
    {
        var resume = await dbContext.Resumes
            .AsNoTracking()
            .Where(candidate => candidate.Id == resumeId && candidate.UserId == userId)
            .Select(candidate => new { candidate.FileName, candidate.FilePath })
            .SingleOrDefaultAsync(cancellationToken);

        if (resume is null)
        {
            return null;
        }

        var contentType = Path.GetExtension(resume.FileName).ToLowerInvariant() switch
        {
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/pdf"
        };

        return new ResumeFileResult(
            storageService.OpenRead(resume.FilePath),
            resume.FileName,
            contentType);
    }

    public async Task<ResumeUploadResult> UploadAsync(
        int userId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var validationError = await ValidateFileAsync(file, cancellationToken);
        if (validationError is not null)
        {
            return ResumeUploadResult.Failure(validationError);
        }

        StoredResumeFile? storedFile = null;

        try
        {
            storedFile = await storageService.SaveAsync(userId, file, cancellationToken);
            var extractedText = parserService.ExtractText(storedFile.AbsolutePath);
            var detectedSkills = await skillExtractionService.ExtractSkillsAsync(
                extractedText,
                cancellationToken);

            var resume = new Resume
            {
                UserId = userId,
                FileName = Path.GetFileName(file.FileName),
                FilePath = storedFile.RelativePath,
                ExtractedText = extractedText
            };

            foreach (var skill in detectedSkills)
            {
                resume.ResumeSkills.Add(new ResumeSkill
                {
                    Resume = resume,
                    Skill = skill,
                    SkillId = skill.Id
                });
            }

            dbContext.Resumes.Add(resume);
            await dbContext.SaveChangesAsync(cancellationToken);

            var responseSkills = detectedSkills
                .Select(ToDetectedSkillResponse)
                .OrderBy(skill => skill.Name)
                .ToList();
            var message = extractedText.Length == 0
                ? "Resume uploaded, but no selectable text was found. Scanned PDFs require OCR."
                : $"Resume uploaded, text extracted, and {responseSkills.Count} skills detected.";

            return ResumeUploadResult.Success(new ResumeUploadResponse(
                resume.Id,
                resume.FileName,
                resume.UploadedAt,
                extractedText.Length,
                responseSkills,
                message));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Resume upload or document parsing failed for user {UserId}.", userId);

            if (storedFile is not null)
            {
                await storageService.DeleteAsync(storedFile.RelativePath);
            }

            return ResumeUploadResult.Failure(
                "The document could not be read. Make sure it is a valid, unprotected PDF or DOCX file.");
        }
    }

    public async Task<ResumeDetailResponse?> ReanalyzeSkillsAsync(
        int userId,
        int resumeId,
        CancellationToken cancellationToken)
    {
        var resume = await dbContext.Resumes
            .Include(candidate => candidate.ResumeSkills)
            .SingleOrDefaultAsync(
                candidate => candidate.Id == resumeId && candidate.UserId == userId,
                cancellationToken);

        if (resume is null)
        {
            return null;
        }

        var detectedSkills = await skillExtractionService.ExtractSkillsAsync(
            resume.ExtractedText ?? string.Empty,
            cancellationToken);

        dbContext.ResumeSkills.RemoveRange(resume.ResumeSkills);
        resume.ResumeSkills = detectedSkills
            .Select(skill => new ResumeSkill
            {
                ResumeId = resume.Id,
                SkillId = skill.Id,
                Resume = resume,
                Skill = skill
            })
            .ToList();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ResumeDetailResponse(
            resume.Id,
            resume.FileName,
            resume.ExtractedText ?? string.Empty,
            resume.UploadedAt,
            detectedSkills.Select(ToDetectedSkillResponse).OrderBy(skill => skill.Name).ToList());
    }

    public async Task<DeleteResumeResult> DeleteAsync(
        int userId,
        int resumeId,
        CancellationToken cancellationToken)
    {
        var resume = await dbContext.Resumes
            .SingleOrDefaultAsync(
                candidate => candidate.Id == resumeId && candidate.UserId == userId,
                cancellationToken);

        if (resume is null)
        {
            return DeleteResumeResult.NotFound;
        }

        var hasMatchHistory = await dbContext.JobMatches
            .AnyAsync(jobMatch => jobMatch.ResumeId == resumeId, cancellationToken);
        if (hasMatchHistory)
        {
            return DeleteResumeResult.HasMatchHistory;
        }

        dbContext.Resumes.Remove(resume);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            await storageService.DeleteAsync(resume.FilePath);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(
                exception,
                "Resume record {ResumeId} was deleted but its stored file could not be removed.",
                resumeId);
        }

        return DeleteResumeResult.Deleted;
    }

    private async Task<string?> ValidateFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return "Choose a non-empty PDF or DOCX file.";
        }

        if (file.Length > _storageOptions.MaximumFileSizeBytes)
        {
            return $"The document must be smaller than {_storageOptions.MaximumFileSizeBytes / (1024 * 1024)} MB.";
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension is not ".pdf" and not ".docx")
        {
            return "Only PDF and DOCX resumes are supported.";
        }

        var expectedSignature = extension == ".pdf" ? PdfSignature : ZipSignature;
        await using var stream = file.OpenReadStream();
        var signature = new byte[expectedSignature.Length];
        var bytesRead = await stream.ReadAsync(signature, cancellationToken);

        if (bytesRead != expectedSignature.Length || !signature.SequenceEqual(expectedSignature))
        {
            return $"The selected file is not a valid {extension[1..].ToUpperInvariant()} document.";
        }

        return null;
    }

    private static DetectedSkillResponse ToDetectedSkillResponse(Skill skill) =>
        new(skill.Id, skill.Name, skill.Category);
}
