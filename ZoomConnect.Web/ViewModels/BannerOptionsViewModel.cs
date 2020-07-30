using System;

namespace ZoomConnect.Web.ViewModels
{
    public class BannerOptionsViewModel
    {
        public string Instance { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string CurrentTerm { get; set; }
        public string CurrentSubject { get; set; }

        public DateTime TermStart { get; set; }
        public DateTime TermEnd { get; set; }

        public string ZoomApiKey { get; set; }
        public string ZoomApiSecret { get; set; }

        public bool UseCanvas { get; set; }
        public string CanvasAccessToken { get; set; }
    }
}
