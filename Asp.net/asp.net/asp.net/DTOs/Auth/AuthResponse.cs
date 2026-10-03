namespace SkillMatch.API.DTOs.Auth;

public sealed record AuthResponse(
    string Token,
    DateTimeOffset ExpiresAt,
    AuthenticatedUser User);

public sealed record AuthenticatedUser(
    int Id,
    string FullName,
    string Email,
    DateTimeOffset CreatedAt);
