using System;

namespace ZoomConnect.Web.Models
{
    public class ZoomOptions
    {
        public ZoomOptions()
        {
            Banner = new BannerOptions();
        }

        public string CurrentTerm { get; set; }
        public string CurrentSubject { get; set; }

        public BannerOptions Banner { get; set; }
        public ZoomApiOptions ZoomApi { get; set; }
    }
}
