using Npgsql;
namespace Hovedopgave.Server.Database
{
    public class PostgreSQL
    {
        private bool uselocaldb { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        public string connectionstring {get; set; }
        public string host { get; set; }
        public string database { get; set; }

        public PostgreSQL(bool uselocaldb) 
        {
            this.uselocaldb = uselocaldb;
            
            // Setup connection
            if (this.uselocaldb) 
            {
                // Connect to local DB
                this.username = DotNetEnv.Env.GetString("LOCAL_DB_USERNAME");
                this.password = DotNetEnv.Env.GetString("LOCAL_DB_PASSWORD");
                this.host = DotNetEnv.Env.GetString("LOCAL_DB_HOST");
                this.database = DotNetEnv.Env.GetString("LOCAL_DB_DATABASE");

                this.connectionstring = $"Host={this.host};Username={this.username};Password={this.password};Database={this.database}";
            }
            else 
            {
                // Connect to azure
                this.username = DotNetEnv.Env.GetString("AZURE_DB_USERNAME");
                this.password = DotNetEnv.Env.GetString("AZURE_DB_PASSWORD");
                this.host = DotNetEnv.Env.GetString("AZURE_DB_HOST");
                this.database = DotNetEnv.Env.GetString("AZURE_DB_DATABASE");

                this.connectionstring = $"Host={this.host};Username={this.username};Password={this.password};Database={this.database};SslMode=Prefer";
            }
        }
    }
}
