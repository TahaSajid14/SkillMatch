using SkillMatch.API.DTOs.Dashboard;

namespace SkillMatch.API.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync(int userId, CancellationToken cancellationToken);
}
