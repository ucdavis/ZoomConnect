using System;
using SecretJsonConfig;

namespace ZoomConnect.Web.Models
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
    }
}
