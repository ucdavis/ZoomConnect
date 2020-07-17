using System;
using SecretJsonConfig;

namespace ZoomConnect.Web.Models
{
    public class BannerOptions
    {
        public BannerOptions()
        {
            Username = new SecretStruct("");
            Password = new SecretStruct("");
        }

        public string Instance { get; set; }
        public SecretStruct Username { get; set; }
        public SecretStruct Password { get; set; }

        public string GetConnectionString()
        {
            return $"Data Source={Instance};User Id={Username.Value};Password={Password.Value};";
        }
    }
}
