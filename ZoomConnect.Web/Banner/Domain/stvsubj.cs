using System;

namespace ZoomConnect.Web.Banner.Domain
{
    /// <summary>
    /// Banner course subjects
    /// </summary>
    public class stvsubj : IBannerTable
    {
        public string code { get; set; }
        public string description { get; set; }
    }
}
