using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Services;
using Hovedopgave.Server.Utils;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Text.Json;

namespace Hovedopgave.Server.Controllers
{
    [ApiController]
    [EnableCors("FrontEndUI")]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async Task<string> Login(LoginAttemptDTO credentials)
        {
            //DatabaseSeeder ds = new DatabaseSeeder();
            //ds.SeedUsers(5000);

            LoginServices LS = new LoginServices();
            try 
            {
                LoginDTO loginDto = await LS.Login(credentials);

                if (loginDto != null) return JsonSerializer.Serialize(loginDto);
            }
            catch (Exception e) 
            {
            
            }

            return string.Empty;
        }
    }
}
