using System;

namespace ZoomConnect.Core.Config
{
    public class ZoomOptions
    {
        public ZoomOptions()
        {
            Banner = new BannerOptions();
            ZoomApi = new ZoomApiOptions();
            CanvasApi = new CanvasApiOptions();
        }

        public string CurrentTerm { get; set; }
        public string CurrentSubject { get; set; }

        public DateTime TermStart { get; set; }
        public DateTime TermEnd { get; set; }

        public BannerOptions Banner { get; set; }
        public ZoomApiOptions ZoomApi { get; set; }
        public CanvasApiOptions CanvasApi { get; set; }
    }
}
