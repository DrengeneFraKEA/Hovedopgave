using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Hovedopgave.Server.Database;
using Hovedopgave.Server.Services;
using Hovedopgave.Server.DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize]
    [HttpGet("totals/overview")]
    public async Task<IActionResult> GetOverviewTotals()
    {
        try 
        {
            TotalCountsDTO totals = new TotalCountsDTO
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
        catch(Exception e) 
        {
            return NotFound();
        }
    }
}