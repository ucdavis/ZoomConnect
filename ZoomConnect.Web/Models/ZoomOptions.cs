using System;

namespace ZoomConnect.Web.Models
{
    public class ZoomOptions
    {
        public ZoomOptions()
        {
            Banner = new BannerOptions();
        }

        public BannerOptions Banner { get; set; }
    }
}
