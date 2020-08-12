using SecretJsonConfig;
using System;

namespace ZoomConnect.Core.Config
{
    public class EmailOptions
    {
        public string smtpHost { get; set; }
        public string username { get; set; }
        public SecretStruct password { get; set; }
    }
}
