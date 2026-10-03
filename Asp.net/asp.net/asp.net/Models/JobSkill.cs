namespace SkillMatch.API.Models;

public class JobSkill
{
    public int JobId { get; set; }
    public int SkillId { get; set; }
    public SkillImportance Importance { get; set; } = SkillImportance.Required;

    public Job Job { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

public enum SkillImportance
{
    Required,
    Preferred,
    Optional
}
