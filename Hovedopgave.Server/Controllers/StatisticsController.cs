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
}