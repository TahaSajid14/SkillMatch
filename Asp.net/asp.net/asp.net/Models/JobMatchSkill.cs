namespace SkillMatch.API.Models;

public class JobMatchSkill
{
    public int JobMatchId { get; set; }
    public int SkillId { get; set; }
    public bool IsMatched { get; set; }

    public JobMatch JobMatch { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
