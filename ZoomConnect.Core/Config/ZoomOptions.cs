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
            EmailOptions = new EmailOptions();
        }

        public string CurrentTerm { get; set; }
        public string CurrentSubject { get; set; }

        public DateTime TermStart { get; set; }
        public DateTime TermEnd { get; set; }

        /// <summary>
        /// Date of last Participant Report.
        /// Updated by participant report service.  Do not display to user.
        /// </summary>
        public DateTime LastParticipantReportDate { get; set; }

        public BannerOptions Banner { get; set; }
        public ZoomApiOptions ZoomApi { get; set; }
        public CanvasApiOptions CanvasApi { get; set; }
        public EmailOptions EmailOptions { get; set; }
    }
}
