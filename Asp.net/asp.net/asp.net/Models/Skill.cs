namespace SkillMatch.API.Models;

public class Skill
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }

    public ICollection<ResumeSkill> ResumeSkills { get; set; } = [];
    public ICollection<JobSkill> JobSkills { get; set; } = [];
    public ICollection<JobMatchSkill> JobMatchSkills { get; set; } = [];
    public ICollection<SkillRecommendation> Recommendations { get; set; } = [];
}
