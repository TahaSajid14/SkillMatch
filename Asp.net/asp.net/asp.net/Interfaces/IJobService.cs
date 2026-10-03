using SkillMatch.API.DTOs.Jobs;

namespace SkillMatch.API.Interfaces;

public interface IJobService
{
    Task<IReadOnlyList<JobSummaryResponse>> GetAllAsync(int userId, CancellationToken cancellationToken);
    Task<JobDetailResponse?> GetByIdAsync(int userId, int jobId, CancellationToken cancellationToken);
    Task<JobDetailResponse> CreateAsync(int userId, JobUpsertRequest request, CancellationToken cancellationToken);
    Task<JobDetailResponse?> UpdateAsync(int userId, int jobId, JobUpsertRequest request, CancellationToken cancellationToken);
    Task<DeleteJobResult> DeleteAsync(int userId, int jobId, CancellationToken cancellationToken);
}

public enum DeleteJobResult
{
    Deleted,
    NotFound,
    HasMatchHistory
}
