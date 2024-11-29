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
        // DELETE THIS
        public Dictionary<string, string> tempAccounts = new Dictionary<string, string>() 
        {
            {"john", "1234" },
        };

        [HttpPost]
        public async Task<string> Login(LoginAttemptDTO credentials)
        {
            //DatabaseSeeder ds = new DatabaseSeeder();
            //ds.SeedTeams(10);

            // This is for temp password generation - ignore
            // string salt = string.Empty;
            // string test = PasswordHandler.GenerateSaltAndHashedPassword(credentials.password, out salt);

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
