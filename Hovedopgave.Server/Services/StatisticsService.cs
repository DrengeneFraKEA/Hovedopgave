using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Utils;
using Microsoft.EntityFrameworkCore;

namespace Hovedopgave.Server.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SignupStatsDTO> GetSignupStats(DateTime? fromDate, DateTime? toDate)
        {
            var now = DateTime.UtcNow;

            // Default to a very early date if fromDate is not provided
            fromDate ??= DateTime.MinValue.ToUniversalTime(); 

            // Default to current UTC time if toDate is not provided
            toDate ??= DateTime.UtcNow; 

            // Calculate start and end of the date ranges for comparison
            var startOfDay = fromDate.Value.Date; 
            var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1); // end of the day (one tick before the next day) nanosecond ticks:)

            // filtering date range 
            var userSignups = StatisticsFilter.ApplyDateRangeFilter(_context.Users, u => u.created_at, startOfDay, endOfDay);
            var teamSignups = StatisticsFilter.ApplyDateRangeFilter(_context.Teams, t => t.created_at, startOfDay, endOfDay);
            var organizationSignups = StatisticsFilter.ApplyDateRangeFilter(_context.Organizations, o => o.created_at, startOfDay, endOfDay);

            // Fetch counts
            var totalUsers = await userSignups.CountAsync();
            var totalTeams = await teamSignups.CountAsync();
            var totalOrganizations = await organizationSignups.CountAsync();

            // Daily signups: filter within today's date range
            var dailyStart = now.Date; // today at midnight
            var dailyEnd = dailyStart.AddDays(1).AddTicks(-1); // end of today (one tick before the next day)

            var dailyUsers = await userSignups.CountAsync(u => u.created_at >= dailyStart && u.created_at <= dailyEnd);
            var dailyTeams = await teamSignups.CountAsync(t => t.created_at >= dailyStart && t.created_at <= dailyEnd);
            var dailyOrganizations = await organizationSignups.CountAsync(o => o.created_at >= dailyStart && o.created_at <= dailyEnd);

            // Weekly signups: filter within the last 7 days
            var weeklyStart = now.AddDays(-7);
            var weeklyUsers = await userSignups.CountAsync(u => u.created_at >= weeklyStart);
            var weeklyTeams = await teamSignups.CountAsync(t => t.created_at >= weeklyStart);
            var weeklyOrganizations = await organizationSignups.CountAsync(o => o.created_at >= weeklyStart);

            // Monthly signups: filter within the last month
            var monthlyStart = now.AddMonths(-1);
            var monthlyUsers = await userSignups.CountAsync(u => u.created_at >= monthlyStart);
            var monthlyTeams = await teamSignups.CountAsync(t => t.created_at >= monthlyStart);
            var monthlyOrganizations = await organizationSignups.CountAsync(o => o.created_at >= monthlyStart);

            // Return stats
            return new SignupStatsDTO
            {
                TotalSignups = totalUsers + totalTeams + totalOrganizations,
                UserSignups = totalUsers,
                TeamSignups = totalTeams,
                OrganizationSignups = totalOrganizations,
                DailySignups = dailyUsers + dailyTeams + dailyOrganizations,
                WeeklySignups = weeklyUsers + weeklyTeams + weeklyOrganizations,
                MonthlySignups = monthlyUsers + monthlyTeams + monthlyOrganizations
            };
        }
    }
}
