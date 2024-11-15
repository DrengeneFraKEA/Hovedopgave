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
        [HttpGet]
        public void Get() 
        {
        
        }


        [HttpPost]
        public bool Login()
        {
            return true;
        }
    }
}
