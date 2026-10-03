using System.ComponentModel.DataAnnotations;

namespace SkillMatch.API.DTOs.Resumes;

public sealed class ResumeUploadRequest
{
    [Required]
    public IFormFile File { get; init; } = null!;
}
