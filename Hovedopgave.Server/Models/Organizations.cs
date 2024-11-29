namespace Hovedopgave.Server.Models
{
    public class Organizations
    {
        public string id {  get; set; }
        public string name { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;
        public DateTime deleted_at { get; set; }

    }
}
