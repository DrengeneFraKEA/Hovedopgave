using static Hovedopgave.Server.Models.Roles;

namespace Hovedopgave.Server.Models
{
    public class Users
    {
        public string id { get; set; }
        public string full_name { get; set; }
        public string display_name { get; set; }
        public Role role { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string password_salt { get; set; }
        public string password { get; set; }
        public int phone_ext { get; set; }
        public string phone { get; set; }
        public string country { get; set; }
        public string discord_id { get; set; }
        public DateTime birthday { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
