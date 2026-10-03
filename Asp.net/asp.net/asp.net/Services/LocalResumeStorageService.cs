using Microsoft.Extensions.Options;
using SkillMatch.API.Configuration;
using SkillMatch.API.Interfaces;

namespace SkillMatch.API.Services;

public sealed class LocalResumeStorageService : IResumeStorageService
{
    private readonly string _storageRoot;

    public LocalResumeStorageService(
        IWebHostEnvironment environment,
        IOptions<ResumeStorageOptions> options)
    {
        _storageRoot = Path.GetFullPath(Path.Combine(
            environment.ContentRootPath,
            options.Value.Directory));
        Directory.CreateDirectory(_storageRoot);
    }

    public async Task<StoredResumeFile> SaveAsync(
        int userId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var userDirectory = Path.Combine(_storageRoot, userId.ToString());
        Directory.CreateDirectory(userDirectory);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(userDirectory, storedFileName);
        var relativePath = Path.GetRelativePath(_storageRoot, absolutePath)
            .Replace(Path.DirectorySeparatorChar, '/');

        await using var destination = new FileStream(
            absolutePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous);
        await file.CopyToAsync(destination, cancellationToken);

        return new StoredResumeFile(relativePath, absolutePath);
    }

    public Stream OpenRead(string relativePath)
    {
        var absolutePath = ResolveSafePath(relativePath);
        return new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public Task DeleteAsync(string relativePath)
    {
        var absolutePath = ResolveSafePath(relativePath);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }

    private string ResolveSafePath(string relativePath)
    {
        var absolutePath = Path.GetFullPath(Path.Combine(_storageRoot, relativePath));
        var requiredPrefix = _storageRoot.EndsWith(Path.DirectorySeparatorChar)
            ? _storageRoot
            : _storageRoot + Path.DirectorySeparatorChar;

        if (!absolutePath.StartsWith(requiredPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The stored resume path is invalid.");
        }

        return absolutePath;
    }
}
