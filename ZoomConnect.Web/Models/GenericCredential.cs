using System;
using System.Text.Json.Serialization;
using SecretJsonConfig;

namespace ZoomConnect.Web.Models
{
    public class GenericCredential
    {
        public string Username { get; set; }
        public SecretString Password { get; set; }
    }
}
