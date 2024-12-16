using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;

namespace Hovedopgave.Server.Services
{
    public interface IGraphService
    {
        Task<GraphDTO[]> GetCustomUsers(string fromDate, string toDate);
        Task<GraphDTO[]> GetCustomTeams(string fromDate, string toDate);
        Task<GraphDTO[]> GetCustomOrganisations(string fromDate, string toDate);

        Task<GraphDTO[]> GetDailyUsers(int daysInThePast);
        Task<GraphDTO[]> GetWeeklyUsers(int weeksInThePast);
        Task<GraphDTO[]> GetMonthlyUsers(int monthsInThePast);

        Task<GraphDTO[]> GetDailyTeams(int daysInThePast);
        Task<GraphDTO[]> GetWeeklyTeams(int weeksInThePast);
        Task<GraphDTO[]> GetMonthlyTeams(int monthsInThePast);

        Task<GraphDTO[]> GetDailyOrganisations(int daysInThePast);
        Task<GraphDTO[]> GetWeeklyOrganisations(int weeksInThePast);
        Task<GraphDTO[]> GetMonthlyOrganisations(int monthsInThePast);
    }
}
