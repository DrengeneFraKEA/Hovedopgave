using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;

namespace Hovedopgave.Server.Services
{
    public interface IGraphService
    {
        Task<GraphDTO[]> GetDailyUsers(int daysInThePast);
        Task<GraphDTO[]> GetWeeklyUsers(int weeksInThePast);
        Task<GraphDTO[]> GetMonthlyUsers(int monthsInThePast);
    }
}
