using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Hovedopgave.Server.Database;
using Hovedopgave.Server.Services;
using Hovedopgave.Server.DTO;
using Microsoft.AspNetCore.Cors;

[ApiController]
[EnableCors("FrontEndUI")]
[Route("statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }



    [HttpGet("signups")]
    public async Task<IActionResult> GetSignupStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        try
        {
            var stats = await _statisticsService.GetSignupStats(fromDate, toDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("totals/overview")]
    public async Task<IActionResult> GetOverviewTotals()
    {
        var totals = new TotalCountsDTO
        {
            TotalUsers = await _statisticsService.GetTotalUsers(),
            TotalTeams = await _statisticsService.GetTotalTeams(),
            TotalOrganizations = await _statisticsService.GetTotalOrganizations(),
            TotalValorantProfiles = await _statisticsService.GetTotalValorantProfiles(),
            TotalUserGameProfiles = await _statisticsService.GetTotalUserGameProfiles(),
            TotalLeagueProfiles = await _statisticsService.GetTotalLeagueProfiles(),
            TotalCompetitions = await _statisticsService.GetTotalCompetitions()
        };

        return Ok(totals);
    }

    [HttpGet("totals/users")]
    public async Task<IActionResult> GetTotalUsers()
    {
        try
        {
            var totalUsers = await _statisticsService.GetTotalUsers();
            return Ok(totalUsers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("totals/teams")]
    public async Task<IActionResult> GetTotalTeams()
    {
        try
        {
            var totalTeams = await _statisticsService.GetTotalTeams();
            return Ok(totalTeams);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("totals/organizations")]
    public async Task<IActionResult> GetTotalOrganizations()
    {
        try
        {
            var totalOrganizations = await _statisticsService.GetTotalOrganizations();
            return Ok(totalOrganizations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("totals/valorant_profiles")]
    public async Task<IActionResult> GetTotalValorantProfiles()
    {
        try
        {
            var totalValorantProfiles = await _statisticsService.GetTotalValorantProfiles();
            return Ok(totalValorantProfiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("totals/user_game_profiles")]
    public async Task<IActionResult> GetTotalUserGameProfiles()
    {
        try
        {
            var totalUserGameProfiles = await _statisticsService.GetTotalUserGameProfiles();
            return Ok(totalUserGameProfiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("totals/league_profiles")]
    public async Task<IActionResult> GetTotalLeagueProfiles()
    {
        try
        {
            var totalLeagueProfiles = await _statisticsService.GetTotalLeagueProfiles();
            return Ok(totalLeagueProfiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("totals/competitions")]
    public async Task<IActionResult> GetTotalCompetitions()
    {
        try
        {
            var totalCompetitions = await _statisticsService.GetTotalCompetitions();
            return Ok(totalCompetitions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}