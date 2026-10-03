using SkillMatch.API.Models;

namespace SkillMatch.API.Interfaces;

public interface ISkillExtractionService
{
    Task<IReadOnlyList<Skill>> ExtractSkillsAsync(string text, CancellationToken cancellationToken);
}
