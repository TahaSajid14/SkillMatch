namespace SkillMatch.API.Models;

public class SkillRecommendation
{
    public int Id { get; set; }
    public int SkillId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public LearningPriority LearningPriority { get; set; } = LearningPriority.Medium;

    public Skill Skill { get; set; } = null!;
}

public enum LearningPriority
{
    Low,
    Medium,
    High
}
