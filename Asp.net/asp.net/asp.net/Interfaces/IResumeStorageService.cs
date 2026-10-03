namespace SkillMatch.API.Interfaces;

public interface IResumeStorageService
{
    Task<StoredResumeFile> SaveAsync(int userId, IFormFile file, CancellationToken cancellationToken);
    Stream OpenRead(string relativePath);
    Task DeleteAsync(string relativePath);
}

public sealed record StoredResumeFile(string RelativePath, string AbsolutePath);
