using System.Configuration;
using System.Diagnostics;

namespace KPI_APP.Helper
{
    public class Settings
    {
        public static int Hours { get { return int.Parse(ConfigurationManager.AppSettings["hours"]); } }
        public static int Minutes { get { return int.Parse(ConfigurationManager.AppSettings["minutes"]); } }
        public static int Interval { get { return int.Parse(ConfigurationManager.AppSettings["Interval"]); } }
        public static int Start { get { return int.Parse(ConfigurationManager.AppSettings["Start"]); } }
        public static int End { get { return int.Parse(ConfigurationManager.AppSettings["End"]); } }
        public static int Close { get { return int.Parse(ConfigurationManager.AppSettings["Close"]); } }
        public static string Smtp { get { return ConfigurationManager.AppSettings["SMTP"]; } }
        public static int Port { get { return int.Parse(ConfigurationManager.AppSettings["Port"]); } }
        public static string Password { get { return ConfigurationManager.AppSettings["Password"]; } }
        public static string Email { get { return ConfigurationManager.AppSettings["Email"]; } }
        public static string Url { get { return ConfigurationManager.AppSettings["Url"]; } }
        public static string CC { get { return ConfigurationManager.AppSettings["CC"]; } }
    }

    public static class Global
    {
        public static ProcessStartInfo ProcessInfo { get; set; }
    }
}