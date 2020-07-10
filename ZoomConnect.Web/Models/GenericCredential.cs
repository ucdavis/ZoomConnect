using System;
using System.Text.Json.Serialization;
using ZoomConnect.Web.Serialization;

namespace ZoomConnect.Web.Models
{
    public class GenericCredential
    {
        public string Username { get; set; }
        [JsonConverter(typeof(SecretStringConverter))]
        public string Password { get; set; }
    }
}
