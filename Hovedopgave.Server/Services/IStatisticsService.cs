using System.Threading.Tasks;

namespace Hovedopgave.Server.Services
{
    public interface IStatisticsService
    {
        Task<int> GetTotalUsers();
        Task<int> GetTotalTeams();
        Task<int> GetTotalOrganizations();
    }
}
