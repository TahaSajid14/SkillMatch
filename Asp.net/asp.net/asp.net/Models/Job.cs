namespace SkillMatch.API.Models;

public class Job
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Title { get; set; }
    public required string Company { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<JobSkill> JobSkills { get; set; } = [];
    public ICollection<JobMatch> JobMatches { get; set; } = [];
}
