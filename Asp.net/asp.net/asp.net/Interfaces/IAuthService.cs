using SkillMatch.API.DTOs.Auth;

namespace SkillMatch.API.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<AuthServiceResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}

public sealed record AuthServiceResult(AuthResponse? Response, string? Error)
{
    public bool Succeeded => Response is not null;

    public static AuthServiceResult Success(AuthResponse response) => new(response, null);
    public static AuthServiceResult Failure(string error) => new(null, error);
}
