//using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;
using ZoomConnect.Web.SecretJsonConfig;
using ZoomConnect.Web.Serialization;

namespace ZoomConnect.Web.Models
{
    public class ZoomOptions : ISecret
    {
        public const string Name = "ZoomOptions";

        //[JsonConverter(typeof(JsonNetSecretStringConverter))]
        [JsonConverter(typeof(SecretStringConverter))]
        public string Secret { get; set; }
        public string NotSecret { get; set; }
    }
}
