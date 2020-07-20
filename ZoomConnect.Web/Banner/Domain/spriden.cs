using System;

namespace ZoomConnect.Web.Banner.Domain
{
    public class spriden : IBannerTable
    {
        public decimal pidm { get; set; }
        public string id { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
    }
}
