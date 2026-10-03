using System.ComponentModel.DataAnnotations;

namespace SkillMatch.API.Configuration;

public sealed class ResumeStorageOptions
{
    public const string SectionName = "ResumeStorage";

    [Required]
    public string Directory { get; init; } = "Storage/Resumes";

    [Range(1024, 20 * 1024 * 1024)]
    public long MaximumFileSizeBytes { get; init; } = 5 * 1024 * 1024;
}
