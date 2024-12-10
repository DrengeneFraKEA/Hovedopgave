using Hovedopgave.Server.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Hovedopgave.Server.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsers()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalTeams()
        {
            return await _context.Teams.CountAsync();
        }

        public async Task<int> GetTotalOrganizations()
        {
            return await _context.Organizations.CountAsync();
        }
    }
}
