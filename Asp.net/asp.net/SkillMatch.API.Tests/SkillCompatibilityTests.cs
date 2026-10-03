using SkillMatch.API.Helpers;
using Xunit;

namespace SkillMatch.API.Tests;

public sealed class SkillCompatibilityTests
{
    [Fact]
    public void AspNetCore_CoversDotNetRequirement()
    {
        var resumeSkills = Skills("ASP.NET Core");

        Assert.True(SkillCompatibility.IsRequirementCovered(".NET", resumeSkills));
    }

    [Fact]
    public void DotNet_DoesNotCoverAspNetCoreRequirement()
    {
        var resumeSkills = Skills(".NET");

        Assert.False(SkillCompatibility.IsRequirementCovered("ASP.NET Core", resumeSkills));
    }

    [Fact]
    public void EntityFrameworkCore_CoversDotNetRequirement()
    {
        var resumeSkills = Skills("Entity Framework Core");

        Assert.True(SkillCompatibility.IsRequirementCovered(".NET", resumeSkills));
    }

    [Fact]
    public void TailwindCss_CoversCssRequirement()
    {
        var resumeSkills = Skills("Tailwind CSS");

        Assert.True(SkillCompatibility.IsRequirementCovered("CSS", resumeSkills));
    }

    [Fact]
    public void UnrelatedSkill_DoesNotCoverRequirement()
    {
        var resumeSkills = Skills("React", "SQL Server");

        Assert.False(SkillCompatibility.IsRequirementCovered("Docker", resumeSkills));
    }

    private static HashSet<string> Skills(params string[] names) =>
        names.ToHashSet(StringComparer.OrdinalIgnoreCase);
}
