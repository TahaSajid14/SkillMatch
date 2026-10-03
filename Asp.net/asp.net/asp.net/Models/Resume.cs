namespace SkillMatch.API.Models;

public class Resume
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public string? ExtractedText { get; set; }
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<ResumeSkill> ResumeSkills { get; set; } = [];
    public ICollection<JobMatch> JobMatches { get; set; } = [];
}
