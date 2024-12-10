namespace Hovedopgave.Server.Models
{
    public class Competitions
    {
        public string id { get; set; }
        public string name { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string owner { get; set; }
        public string game { get; set; }
        public bool publish { get; set; }
        public string sponsor { get; set; }
        public int team_size { get; set; }
        public bool public_signup { get; set; }
        public DateTime signup_start_date { get; set; }
        public DateTime signup_end_date { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public bool self_report { get; set; }
        public string discord_server { get; set; }
    }
}
