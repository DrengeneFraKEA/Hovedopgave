namespace Hovedopgave.Server.Models
{
    public class Teams
    {
        public string id { get; set; }
        public string name { get; set; }
        public string initials { get; set; }
        public string game { get; set; }
        public string country{ get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;
        public DateTime deleted_at { get; set; }
    }
}
