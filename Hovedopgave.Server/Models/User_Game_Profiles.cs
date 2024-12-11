namespace Hovedopgave.Server.Models
{
    public class User_Game_Profiles
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string profile_id { get; set; }
        public string profile_type { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
