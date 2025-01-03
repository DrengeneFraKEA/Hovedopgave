using static Hovedopgave.Server.Models.Roles;

namespace Hovedopgave.Server.DTO
{
    public class LoginDTO
    {
        public string user_id { get; set; }
        public string username { get; set; }
        public string token { get; set; }
        public string error { get; set; }
    }
}
