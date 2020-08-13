using SecretJsonConfig;
using System;

namespace ZoomConnect.Core.Config
{
    public class EmailOptions
    {
        public string SmtpHost { get; set; }
        public string Username { get; set; }
        public SecretStruct Password { get; set; }

        /// <summary>
        /// comma-separated list of email addresses to cc: on Participant Reports
        /// </summary>
        public string ParticipantReportCcList { get; set; }
    }
}
