using System;

namespace ZoomConnect.Web.Banner.Domain
{
    /// <summary>
    /// Calendar of holidays
    /// </summary>
    public class sobcald : IBannerTable
    {
        public DateTime date { get; set; }
        public string dayt_code { get; set; }
    }
}
