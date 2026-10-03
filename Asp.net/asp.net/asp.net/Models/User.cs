namespace SkillMatch.API.Models;

public class User
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Resume> Resumes { get; set; } = [];
    public ICollection<Job> Jobs { get; set; } = [];
    public ICollection<JobMatch> JobMatches { get; set; } = [];
}
