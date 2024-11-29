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
        Assert.Equal(4, result.WeeklySignups); // 2 users, 1 team, 1 org within last 7 days
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
}
