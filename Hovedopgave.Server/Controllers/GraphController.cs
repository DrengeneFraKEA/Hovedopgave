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
}
