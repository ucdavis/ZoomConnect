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
    }
}
