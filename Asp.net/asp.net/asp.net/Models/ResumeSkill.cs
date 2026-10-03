namespace SkillMatch.API.Models;

public class ResumeSkill
{
    public int ResumeId { get; set; }
    public int SkillId { get; set; }

    public Resume Resume { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
