using System;
using SecretJsonConfig;

namespace ZoomConnect.Core.Config
{
    public class CanvasApiOptions
    {
        public CanvasApiOptions()
        {
            ApiAccessToken = new SecretStruct("");
        }

        public bool UseCanvas { get; set; }
        public SecretStruct ApiAccessToken { get; set; }
        public int EnrollmentTerm { get; set; }
    }
}
