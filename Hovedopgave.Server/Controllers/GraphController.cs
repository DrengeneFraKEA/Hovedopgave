using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Services;
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

    [HttpGet("custom/users")]
    public async Task<GraphDTO[]> GetCustomUsers(string fromDate, string toDate)
    {
        try
        {
            return await _graphService.GetCustomUsers(fromDate, toDate);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("custom/teams")]
    public async Task<GraphDTO[]> GetCustomTeams(string fromDate, string toDate)
    {
        try
        {
            return await _graphService.GetCustomTeams(fromDate, toDate);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("custom/organisations")]
    public async Task<GraphDTO[]> GetCustomOrganisations(string fromDate, string toDate)
    {
        try
        {
            return await _graphService.GetCustomOrganisations(fromDate, toDate);
        }
        catch (Exception e)
        {

        }

        return null;
    }


    [HttpGet("daily/users")]
    public async Task<GraphDTO[]> GetDailyUsers() 
    {
        try 
        {
            return await _graphService.GetDailyUsers(14);
        }
        catch(Exception e) 
        {
            
        }

        return null;
    }

    [HttpGet("weekly/users")]
    public async Task<GraphDTO[]> GetWeeklyUsers()
    {
        try
        {
            return await _graphService.GetWeeklyUsers(12);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("monthly/users")]
    public async Task<GraphDTO[]> GetMonthlyUsers()
    {
        try
        {
            return await _graphService.GetMonthlyUsers(12);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("daily/teams")]
    public async Task<GraphDTO[]> GetDailyTeams()
    {
        try
        {
            return await _graphService.GetDailyTeams(14);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("weekly/teams")]
    public async Task<GraphDTO[]> GetWeeklyTeams()
    {
        try
        {
            return await _graphService.GetWeeklyTeams(12);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("monthly/teams")]
    public async Task<GraphDTO[]> GetMonthlyTeams()
    {
        try
        {
            return await _graphService.GetMonthlyTeams(12);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("daily/organisations")]
    public async Task<GraphDTO[]> GetDailyOrganisations()
    {
        try
        {
            return await _graphService.GetDailyOrganisations(14);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("weekly/organisations")]
    public async Task<GraphDTO[]> GetWeeklyOrganisations()
    {
        try
        {
            return await _graphService.GetWeeklyOrganisations(12);
        }
        catch (Exception e)
        {

        }

        return null;
    }

    [HttpGet("monthly/organisations")]
    public async Task<GraphDTO[]> GetMonthlyOrganisations()
    {
        try
        {
            return await _graphService.GetMonthlyOrganisations(12);
        }
        catch (Exception e)
        {

        }

        return null;
    }
}
