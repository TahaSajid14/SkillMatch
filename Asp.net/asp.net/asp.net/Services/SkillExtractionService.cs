using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Data;
using SkillMatch.API.Interfaces;
using SkillMatch.API.Models;

namespace SkillMatch.API.Services;

public sealed class SkillExtractionService(SkillMatchDbContext dbContext) : ISkillExtractionService
{
    private static readonly IReadOnlyDictionary<string, string[]> Aliases =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["JavaScript"] = ["ECMAScript"],
            ["C#"] = ["C Sharp"],
            ["ASP.NET Core"] = ["ASP.NET", "ASP Net Core", "ASP.NETCORE", "ASP .NET Core"],
            ["Entity Framework Core"] = ["Entity Framework", "EF Core"],
            ["REST API"] = ["REST APIs", "RESTful API", "RESTful APIs"],
            ["SQL Server"] = ["Microsoft SQL Server", "MSSQL"],
            ["PostgreSQL"] = ["Postgres"],
            ["MongoDB"] = ["Mongo DB"],
            ["GitHub"] = ["Git Hub"],
            ["Kubernetes"] = ["K8s"],
            ["AWS"] = ["Amazon Web Services"],
            ["Unit Testing"] = ["Unit Tests", "Unit Test"],
            ["JWT"] = ["JSON Web Token", "JSON Web Tokens"],
            ["Vue.js"] = ["VueJS", "Vue JS"],
            ["Node.js"] = ["NodeJS", "Node JS"],
            ["xUnit"] = ["xUnit.net"],
            ["CI/CD"] = ["Continuous Integration", "Continuous Delivery", "Continuous Deployment"],
            [".NET"] = ["dotnet"],
            ["Next.js"] = ["NextJS", "Next JS"],
            ["Tailwind CSS"] = ["TailwindCSS"],
            ["GraphQL"] = ["Graph QL"]
        };

    public async Task<IReadOnlyList<Skill>> ExtractSkillsAsync(
        string text,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var skills = await dbContext.Skills
            .OrderBy(skill => skill.Name)
            .ToListAsync(cancellationToken);

        return skills
            .Where(skill => GetSearchTerms(skill).Any(term => ContainsTerm(text, term, skill.Name)))
            .ToList();
    }

    private static IEnumerable<string> GetSearchTerms(Skill skill)
    {
        yield return skill.Name;

        if (Aliases.TryGetValue(skill.Name, out var aliases))
        {
            foreach (var alias in aliases)
            {
                yield return alias;
            }
        }
    }

    private static bool ContainsTerm(string text, string term, string skillName)
    {
        var compoundExclusion = skillName.Equals("CSS", StringComparison.OrdinalIgnoreCase)
            ? @"(?<!Tailwind\s)"
            : string.Empty;
        var pattern = $@"{compoundExclusion}(?<![\p{{L}}\p{{N}}]){Regex.Escape(term)}(?![\p{{L}}\p{{N}}])";
        return Regex.IsMatch(
            text,
            pattern,
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(250));
    }
}
