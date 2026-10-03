namespace SkillMatch.API.Models;

public class JobMatch
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ResumeId { get; set; }
    public int JobId { get; set; }
    public decimal MatchScore { get; set; }
    public int MatchedSkillsCount { get; set; }
    public int MissingSkillsCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = null!;
    public Resume Resume { get; set; } = null!;
    public Job Job { get; set; } = null!;
    public ICollection<JobMatchSkill> MatchSkills { get; set; } = [];
}
