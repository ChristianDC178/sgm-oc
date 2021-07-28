namespace Sgm.OC.Framework
{
    public class Settings
    {
        public static string ConnectionString { get; set; }
        public static string FilePath { get; set; }
        public static string SMTPServerAddress { get; set; }
        public static int SMTPServerPort { get; set; }
        public static bool SMTPServerUsesSSL { get; set; }
        public static string SMTPServerUsername { get; set; }
        public static string SMTPServerPassword { get; set; }
    }
}
