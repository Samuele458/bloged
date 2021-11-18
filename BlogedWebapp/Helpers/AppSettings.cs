using System;

namespace BlogedWebapp.Helpers
{
    /// <summary>
    ///  App settings
    /// </summary>
    public class AppSettings
    {
        public string JwtSecret { get; set; }

        public TimeSpan JwtExpiryTimeFrame { get; set; }

        public string  ConnectionString { get; set; }

    }
}
