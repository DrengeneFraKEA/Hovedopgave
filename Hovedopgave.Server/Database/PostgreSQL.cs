using Npgsql;

namespace Hovedopgave.Server.Database
{
    public class PostgreSQL
    {
        private bool uselocaldb { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        private string port { get; set; } = "5432";
        public string connectionstring {get; set; }

        public PostgreSQL(bool uselocaldb) 
        {
            this.uselocaldb = uselocaldb;


            // Setup connection
            if (this.uselocaldb) 
            {
                // Connect to local DB
                this.username = "postgres"; // ignore for now
                this.password = "1234"; // ignore for now

                this.connectionstring = $"Host=localhost;Username={this.username};Password={this.password};Database=postgres";
            }
            else 
            {
                // Connect to azure
            }
        }
    }
}
