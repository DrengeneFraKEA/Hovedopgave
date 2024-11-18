using Hovedopgave.Server.DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public bool Login(LoginDTO credentials)
        {
            foreach (var element in tempAccounts) if (element.Key == credentials.username && element.Value == credentials.password) return true;
            
            return false;
        }
    }
}
