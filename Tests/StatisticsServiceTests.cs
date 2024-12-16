using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;
using Hovedopgave.Server.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Hovedopgave.Server.Models.Roles;

public class StatisticsServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly StatisticsService _service;

    public StatisticsServiceTests()
    {
        // Create an in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        // Seed initial data
        SeedTestData();

        _service = new StatisticsService(_context);
    }

    // Mock ID generator
    private string GenerateId(string prefix) => $"{prefix}_{Guid.NewGuid().ToString("N")}";

    private void SeedTestData()
    {
        // Add Users with realistic data
        _context.Users.AddRange(
            new Users
            {
                id = GenerateId("usr"),
                full_name = "Song Su-hyeong",
                display_name = "Quad",
                role = Role.USER,
                gender = "female",
                email = "quad@leagues.gg",
                password_salt = "52068387b79b8950960c9546db83bc8be8b12b179f8e17366a983ff49f63d64233f1a080e23498783416ab6ba3d35d622a260606afb4931ab8e3ae91da6fee1aab59c0ca8b55dcfb55f24ca9313914aa942e9080a735340250f76709df27ff684cd115f129cd869537a5a6a586f958689dcfc16e9f1d9d4eb848cebb3e2b19",
                password = "1a2503c973b03850bc439b71b781b79d53b125050ddc0de365bc8a726a0a5ade87d8fed812594b6b890b5e973510aeb8beebc3cccc985403674b0bc8458db152",
                phone_ext = 7383,
                phone = "1236",
                country = "KR",
                discord_id = "149550729126871040",
                birthday = new DateTime(1996, 1, 30),
                created_at = DateTime.UtcNow.AddDays(-1), // Yesterday
                updated_at = DateTime.UtcNow
            },
            new Users
            {
                id = GenerateId("usr"),
                full_name = "Jane Doe",
                display_name = "JaneD",
                role = Role.USER,
                gender = "female",
                email = "janedoe@example.com",
                password_salt = "randomsalt",
                password = "randomhash",
                phone_ext = 123,
                phone = "4567",
                country = "US",
                discord_id = "1234567890",
                birthday = new DateTime(1995, 5, 15),
                created_at = DateTime.UtcNow, // Today
                updated_at = DateTime.UtcNow
            }
        );

        // Add Teams with all required fields
        _context.Teams.AddRange(
            new Teams
            {
                id = GenerateId("tea"),
                name = "Team Liquid",
                initials = "TL",
                game = "league-of-legends",
                country = "US",
                created_at = DateTime.UtcNow.AddDays(-7), // 7 days ago
                updated_at = DateTime.UtcNow
            },
            new Teams
            {
                id = GenerateId("tea"),
                name = "Cloud 9",
                initials = "C9",
                game = "valorant",
                country = "CA",
                created_at = DateTime.UtcNow.AddMonths(-2), // 2 months ago
                updated_at = DateTime.UtcNow
            }
        );

        // Add Organizations with all required fields
        _context.Organizations.AddRange(
            new Organizations
            {
                id = GenerateId("org"),
                name = "Monkeys",
                region = "NA",
                country = "US",
                summary = "Monkeys is a professional esports organization based in the United States.",
                description = "Monkeys is a professional esports organization based in the United States.",
                created_at = DateTime.UtcNow, // Today
                updated_at = DateTime.UtcNow
            },
            new Organizations
            {
                id = GenerateId("org"),
                name = "Lions",
                region = "EU",
                country = "DE",
                summary = "Lions is an elite gaming organization.",
                description = "Lions specializes in strategy games.",
                created_at = DateTime.UtcNow.AddMonths(-3), // 3 months ago
                updated_at = DateTime.UtcNow
            }
        );

        _context.SaveChanges();
    }


    [Fact]
    public async Task GetSignupStats_ShouldReturnCorrectStatsForDailyFilter()
    {
        // Arrange
        var now = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetSignupStats(now, now);

        // Assert
        Assert.Equal(2, result.DailySignups); // 1 user and 1 organization today
        Assert.Equal(1, result.UserSignups);
        Assert.Equal(0, result.TeamSignups);
        Assert.Equal(1, result.OrganizationSignups);
    }

    [Fact]
    public async Task GetSignupStats_ShouldReturnCorrectStatsForWeeklyFilter()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var startOfWeek = now.AddDays(-7);

        // Act
        var result = await _service.GetSignupStats(startOfWeek, now);

        // Assert
        Assert.Equal(3, result.WeeklySignups); // 2 users, 1 team, 1 org within last 7 days
        Assert.Equal(2, result.UserSignups);
        Assert.Equal(1, result.TeamSignups);
        Assert.Equal(1, result.OrganizationSignups);
    }

    [Fact]
    public async Task GetSignupStats_ShouldReturnCorrectStatsForMonthlyFilter()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var startOfMonth = now.AddMonths(-1);

        // Act
        var result = await _service.GetSignupStats(startOfMonth, now);

        // Assert
        Assert.Equal(4, result.MonthlySignups); // 2 users, 1 team, 1 org within last month
        Assert.Equal(2, result.UserSignups);
        Assert.Equal(1, result.TeamSignups);
        Assert.Equal(1, result.OrganizationSignups);
    }

    [Fact]
    public async Task GetSignupStats_ShouldHandleNullDates()
    {
        // Act
        var result = await _service.GetSignupStats(null, null);

        // Assert
        Assert.Equal(6, result.TotalSignups); // All users, teams, and organizations
        Assert.Equal(2, result.UserSignups);
        Assert.Equal(2, result.TeamSignups);
        Assert.Equal(2, result.OrganizationSignups);
    }

    [Fact]
    public async Task GetSignupStats_ShouldReturnZeroWhenNoData()
    {
        // Arrange
        _context.Users.RemoveRange(_context.Users);
        _context.Teams.RemoveRange(_context.Teams);
        _context.Organizations.RemoveRange(_context.Organizations);
        _context.SaveChanges();

        // Act
        var result = await _service.GetSignupStats(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

        // Assert
        Assert.Equal(0, result.TotalSignups);
        Assert.Equal(0, result.UserSignups);
        Assert.Equal(0, result.TeamSignups);
        Assert.Equal(0, result.OrganizationSignups);
    }

    [Fact]
    public async Task GetTotalUsers_ShouldReturnCorrectCount()
    {
        // Act
        var result = await _service.GetTotalUsers();

        // Assert
        Assert.Equal(2, result); // 2 users seeded
    }

    [Fact]
    public async Task GetTotalTeams_ShouldReturnCorrectCount()
    {
        // Act
        var result = await _service.GetTotalTeams();

        // Assert
        Assert.Equal(2, result); // 2 teams seeded
    }

    [Fact]
    public async Task GetTotalOrganizations_ShouldReturnCorrectCount()
    {
        // Act
        var result = await _service.GetTotalOrganizations();

        // Assert
        Assert.Equal(2, result); // 2 organizations seeded
    }



    [Fact]
    public async Task GetSignupStats_ShouldReturnZeroWhenDatesOutOfRange()
    {
        // Act
        var result = await _service.GetSignupStats(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(-1).AddMonths(1));

        // Assert
        Assert.Equal(0, result.TotalSignups);
        Assert.Equal(0, result.UserSignups);
        Assert.Equal(0, result.TeamSignups);
        Assert.Equal(0, result.OrganizationSignups);
    }

    [Fact]
    public async Task GetSignupStats_ShouldThrowExceptionOnInvalidDateRange()
    {
        // Arrange
        var fromDate = DateTime.UtcNow;
        var toDate = DateTime.UtcNow.AddDays(-1); // Invalid range

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetSignupStats(fromDate, toDate));
        Assert.Equal("fromDate cannot be greater than toDate.", exception.Message);
    }


    [Fact]
    public async Task GetSignupStats_ShouldHandleNullContextGracefully()
    {
        // Arrange
        var invalidService = new StatisticsService(null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => invalidService.GetSignupStats(null, null));
    }

    [Fact]
    public async Task GetTotalUsers_ShouldReturnZeroWhenNoUsersExist()
    {
        // Arrange
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTotalUsers();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetTotalTeams_ShouldThrowExceptionWhenDatabaseUnavailable()
    {
        // Arrange
        var invalidContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .Options; // Missing connection
        var invalidService = new StatisticsService(new ApplicationDbContext(invalidContextOptions));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => invalidService.GetTotalTeams());
    }

    [Fact]
    public async Task GetTotalOrganizations_ShouldHandleEmptyDatabase()
    {
        // Arrange
        _context.Organizations.RemoveRange(_context.Organizations);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTotalOrganizations();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetSignupStats_ShouldHandleFutureDatesGracefully()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddYears(1);

        // Act
        var result = await _service.GetSignupStats(futureDate, futureDate.AddDays(1));

        // Assert
        Assert.Equal(0, result.TotalSignups);
        Assert.Equal(0, result.UserSignups);
        Assert.Equal(0, result.TeamSignups);
        Assert.Equal(0, result.OrganizationSignups);
    }

    /* Lige nu tjekker metoderne ikke om users er deleted.
    [Fact]
    public async Task GetSignupStats_ShouldReturnOnlyActiveData()
    {
        // Arrange
        _context.Users.Add(new Users
        {
            id = GenerateId("usr"),
            full_name = "Inactive User",
            display_name = "JaneD",
            role = Role.USER,
            gender = "female",
            email = "janedoe@example.com",
            password_salt = "randomsalt",
            password = "randomhash",
            phone_ext = 123,
            phone = "4567",
            country = "US",
            discord_id = "1234567890",
            birthday = new DateTime(1995, 5, 15),
            created_at = DateTime.UtcNow.AddMonths(-1),
            deleted_at = DateTime.UtcNow.AddDays(-1) // Deleted yesterday
        });

        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetSignupStats(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

        // Assert
        Assert.Equal(2, result.UserSignups); // Only 2 active users
    }
    */

    [Fact]
    public async Task GetSignupStats_ShouldReturnCorrectCountsForPartialFilters()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var yesterday = now.AddDays(-1);

        // Act
        var result = await _service.GetSignupStats(yesterday, now);

        // Assert
        Assert.Equal(2, result.UserSignups); 
        Assert.Equal(0, result.TeamSignups);
        Assert.Equal(1, result.OrganizationSignups);
    }

    [Theory]
    [InlineData("2024-01-01", "2024-01-31", true, null)] // Valid range
    [InlineData("2024-01-01", "2024-01-01", true, null)] // Same day range
    [InlineData("2024-01-01", null, true, null)]         // Only fromDate provided
    [InlineData(null, "2024-01-01", true, null)]         // Only toDate provided
    [InlineData("2024-01-31", "2024-01-01", false, "fromDate cannot be greater than toDate.")] // Invalid range
    [InlineData(null, null, true, null)]                 // Both dates null
    [InlineData("2025-01-01", "2025-01-31", true, null)] // Future dates
    [InlineData("2024-01-01", "2024-12-31", true, null)] // Large valid range
    [InlineData("2023-12-31", "2024-01-01", true, null)] // Overlapping range
    public async Task GetSignupStats_ShouldHandleDateRanges(
    string fromDateStr,
    string toDateStr,
    bool shouldSucceed,
    string expectedErrorMessage)
    {
        // Arrange
        DateTime? fromDate = string.IsNullOrEmpty(fromDateStr) ? null : DateTime.Parse(fromDateStr).ToUniversalTime();
        DateTime? toDate = string.IsNullOrEmpty(toDateStr) ? null : DateTime.Parse(toDateStr).ToUniversalTime();

        if (shouldSucceed)
        {
            // Act
            var result = await _service.GetSignupStats(fromDate, toDate);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.TotalSignups >= 0); // Ensure total signups are non-negative
        }
        else
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetSignupStats(fromDate, toDate));
            Assert.Equal(expectedErrorMessage, exception.Message);
        }
    }

}
