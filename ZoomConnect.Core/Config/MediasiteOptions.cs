using SecretJsonConfig;
using System;

namespace ZoomConnect.Core.Config
{
    public class MediasiteOptions
    {
        public bool UseMediasite { get; set; }

        public string Endpoint { get; set; }
        public string RootFolder { get; set; }
        public string Username { get; set; }
        public SecretStruct Password { get; set; }
        public SecretStruct ApiKey { get; set; }

        public string PlayerId { get; set; }
        public string TemplateId { get; set; }

        public string UploadDirectory { get; set; }
        public string ReportToEmail { get; set; }
        public string ReportReplyToEmail { get; set; }
    }
}
