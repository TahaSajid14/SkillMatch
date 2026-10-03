using SkillMatch.API.DTOs.Resumes;

namespace SkillMatch.API.Interfaces;

public interface IResumeService
{
    Task<IReadOnlyList<ResumeSummaryResponse>> GetAllAsync(int userId, CancellationToken cancellationToken);
    Task<ResumeDetailResponse?> GetByIdAsync(int userId, int resumeId, CancellationToken cancellationToken);
    Task<ResumeFileResult?> OpenFileAsync(int userId, int resumeId, CancellationToken cancellationToken);
    Task<ResumeUploadResult> UploadAsync(int userId, IFormFile file, CancellationToken cancellationToken);
    Task<ResumeDetailResponse?> ReanalyzeSkillsAsync(int userId, int resumeId, CancellationToken cancellationToken);
    Task<DeleteResumeResult> DeleteAsync(int userId, int resumeId, CancellationToken cancellationToken);
}

public sealed record ResumeUploadResult(ResumeUploadResponse? Response, string? Error)
{
    public bool Succeeded => Response is not null;
    public static ResumeUploadResult Success(ResumeUploadResponse response) => new(response, null);
    public static ResumeUploadResult Failure(string error) => new(null, error);
}

public sealed record ResumeFileResult(Stream Stream, string FileName, string ContentType);

public enum DeleteResumeResult
{
    Deleted,
    NotFound,
    HasMatchHistory
}
