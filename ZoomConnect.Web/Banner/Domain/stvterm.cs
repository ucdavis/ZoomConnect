using System;

namespace ZoomConnect.Web.Banner.Domain
{
    public class stvterm : IBannerTable
    {
        public string code { get; set; }
        public string desc { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}
