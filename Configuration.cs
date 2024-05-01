namespace Blog
{
    public static class Configuration
    {
        public static string ApiUrl = null!;
        public static string JwtKey = null!;
        public static string ApiKeyName = null!;
        public static string ApiKey = null!;
        public static SmtpConfiguration Smtp = new();
        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }

}
