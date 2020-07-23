using System;

namespace ZoomConnect.Core.Config
{
    public class ZoomOptions
    {
        public ZoomOptions()
        {
            Banner = new BannerOptions();
            ZoomApi = new ZoomApiOptions();
        }

        public string CurrentTerm { get; set; }
        public string CurrentSubject { get; set; }

        public BannerOptions Banner { get; set; }
        public ZoomApiOptions ZoomApi { get; set; }
    }
}
