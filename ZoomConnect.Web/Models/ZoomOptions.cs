using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SecretJsonConfig;

namespace ZoomConnect.Web.Models
{
    public class ZoomOptions
    {
        public ZoomOptions()
        {
            Secret = new SecretStruct("");
        }

        public SecretStruct Secret { get; set; }
        public string NotSecret { get; set; }
    }
}
