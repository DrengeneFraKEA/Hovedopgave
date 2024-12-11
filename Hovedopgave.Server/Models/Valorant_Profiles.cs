namespace Hovedopgave.Server.Models
{
    public class Valorant_Profiles
    {
        public string puuid { get; set; }
        public string internal_puuid { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public int level { get; set; }
        public string card { get; set; }
        public string title { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string id { get; set; }
    }
}
