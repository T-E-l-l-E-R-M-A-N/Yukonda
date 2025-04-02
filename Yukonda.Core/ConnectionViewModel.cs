namespace Yukonda.Core
{
    public class ConnectionViewModel : BindableBase
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public DbProvider DbProvider { get; set; }
        public List<DbProvider> Providers {get;set;} = new List<DbProvider>()
        {
            new DbProvider() {Name = "SQLite"},
            new DbProvider() {Name = "PostgreSQL"},
        };
    }
}