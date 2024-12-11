namespace Hovedopgave.Server.Models
{
    public class League_of_Legends_Profiles
    {
        public string puuid { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public int level { get; set; }
        public int icon_id { get; set; }
        public string region_id { get; set; }
        public int solo_points { get; set; }
        public string solo_rank { get; set; }
        public string solo_tier { get; set; }
        public int flex_points { get; set; }
        public string flex_tier { get; set; }
        public string flex_rank { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string id { get; set; }
    }
}
