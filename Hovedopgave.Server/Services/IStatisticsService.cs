using System.Threading.Tasks;
using Hovedopgave.Server.DTO;

namespace Hovedopgave.Server.Services
{
    public interface IStatisticsService
    {
        Task<SignupStatsDTO> GetSignupStats(DateTime? fromDate, DateTime? toDate);
        Task<int> GetTotalUsers();
        Task<int> GetTotalTeams();
        Task<int> GetTotalOrganizations();
        Task<int> GetTotalValorantProfiles();
        Task<int> GetTotalUserGameProfiles();
        Task<int> GetTotalLeagueProfiles();
        Task<int> GetTotalCompetitions();

    }
}
