using SkillMatch.API.DTOs.Matches;

namespace SkillMatch.API.Interfaces;

public interface IJobMatchingService
{
    Task<AnalyzeMatchResult> AnalyzeAsync(int userId, AnalyzeMatchRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<MatchHistoryResponse>> GetHistoryAsync(int userId, CancellationToken cancellationToken);
    Task<MatchResultResponse?> GetByIdAsync(int userId, int matchId, CancellationToken cancellationToken);
}

public sealed record AnalyzeMatchResult(MatchResultResponse? Response, string? Error)
{
    public bool Succeeded => Response is not null;
    public static AnalyzeMatchResult Success(MatchResultResponse response) => new(response, null);
    public static AnalyzeMatchResult Failure(string error) => new(null, error);
}
