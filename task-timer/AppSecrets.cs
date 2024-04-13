namespace task_timer
{
    public class AppSecrets
    {
        // database
        public string host {  get; set; }
        public int port { get; set; }
        public string database { get; set; }
        public string key { get; set; }
        public string password { get; set; }
        
        // jwt
        public string issuer { get; set; }
        public string audience { get; set; }
        public string secretKey { get; set; }

  
    }
}
