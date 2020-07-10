using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SecretJsonConfig;

namespace ZoomConnect.Web.Models
{
    public class ZoomOptions
    {
        public const string Name = "ZoomSecrets";

        public ZoomOptions()
        {
            Creds = new List<GenericCredential>();
        }

        [JsonConverter(typeof(SecretStringConverter))]
        public string Secret { get; set; }
        public string NotSecret { get; set; }
        public List<GenericCredential> Creds { get; set; }
    }
}
