using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[EnableCors("FrontEndUI")]
[Route("[controller]")]
public class GraphController : ControllerBase
{
    private readonly IGraphService _graphService;
    public GraphController(IGraphService graphService)
    {
        _graphService = graphService;
    }

    [Authorize]
    [HttpGet("custom")]
    public async Task<GraphDTO[]> GetCustomUsers(string fromDate, string toDate, string type)
    {
        try
        {
            return await _graphService.GetCustomGraphData(fromDate, toDate, type);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [Authorize]
    [HttpGet("daily/users")]
    public async Task<GraphDTO[]> GetDailyUsers() 
    {
        try 
        {
            return await _graphService.GetDailyUsers(30);
        }
        catch(Exception e) 
        {
            
        }

        return null;
    }

    [Authorize]
    [HttpGet("weekly/users")]
    public async Task<GraphDTO[]> GetWeeklyUsers()
    {
        try
        {
            return await _graphService.GetWeeklyUsers(26);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [Authorize]
    [HttpGet("monthly/users")]
    public async Task<GraphDTO[]> GetMonthlyUsers()
    {
        try
        {
            return await _graphService.GetMonthlyUsers(60);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [Authorize]
    [HttpGet("daily/teams")]
    public async Task<GraphDTO[]> GetDailyTeams()
    {
        try
        {
            return await _graphService.GetDailyTeams(30);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [Authorize]
    [HttpGet("weekly/teams")]
    public async Task<GraphDTO[]> GetWeeklyTeams()
    {
        try
        {
            return await _graphService.GetWeeklyTeams(26);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [Authorize]
    [HttpGet("monthly/teams")]
    public async Task<GraphDTO[]> GetMonthlyTeams()
    {
        try
        {
            return await _graphService.GetMonthlyTeams(60);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [Authorize]
    [HttpGet("daily/organisations")]
    public async Task<GraphDTO[]> GetDailyOrganisations()
    {
        try
        {
            return await _graphService.GetDailyOrganisations(30);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [Authorize]
    [HttpGet("weekly/organisations")]
    public async Task<GraphDTO[]> GetWeeklyOrganisations()
    {
        try
        {
            return await _graphService.GetWeeklyOrganisations(26);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [Authorize]
    [HttpGet("monthly/organisations")]
    public async Task<GraphDTO[]> GetMonthlyOrganisations()
    {
        try
        {
            return await _graphService.GetMonthlyOrganisations(60);
        }
        catch (Exception e)
        {

        }

        return null;
    }
}
