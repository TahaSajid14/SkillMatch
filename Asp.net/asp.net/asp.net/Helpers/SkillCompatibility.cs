namespace SkillMatch.API.Helpers;

public static class SkillCompatibility
{
    private static readonly IReadOnlyDictionary<string, string[]> RequirementCoverage =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [".NET"] = ["ASP.NET Core", "Entity Framework Core"],
            ["CSS"] = ["Tailwind CSS"]
        };

    public static bool IsRequirementCovered(
        string requiredSkill,
        IReadOnlySet<string> resumeSkills)
    {
        if (resumeSkills.Contains(requiredSkill))
        {
            return true;
        }

        return RequirementCoverage.TryGetValue(requiredSkill, out var compatibleSkills)
            && compatibleSkills.Any(resumeSkills.Contains);
    }
}
