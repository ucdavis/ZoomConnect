using System;
using SecretJsonConfig;

namespace ZoomConnect.Core.Config
{
    public class ZoomApiOptions
    {
        public ZoomApiOptions()
        {
            ApiSecret = new SecretStruct("");
            ApiKey = new SecretStruct("");
        }

        public SecretStruct ApiSecret { get; set; }
        public SecretStruct ApiKey { get; set; }

        /// <summary>
        /// Whether to turn on Require Auth for all meetings created in Zoom
        /// </summary>
        public bool RequireMeetingAuthentication { get; set; }
        /// <summary>
        /// Id of AuthenticationOption specifying which domains are allowed (optional).
        /// </summary>
        public string AuthenticationOptionId { get; set; }
        /// <summary>
        /// String of comma-separated domains to allow when Require Auth is turned on (optional).
        /// </summary>
        public string AuthenticationDomains { get; set; }
        /// <summary>
        /// String of comma-separated email addresses to serve as alternate hosts to all created meetings.
        /// </summary>
        public string AlternateHosts { get; set; }
    }
}
