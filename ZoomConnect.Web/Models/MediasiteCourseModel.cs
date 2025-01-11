using System.Collections.Generic;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Models
{
    public class MediasiteCourseModel
    {
        public ssrmeet bannerMeeting { get; set; }
        public ssbsect bannerCourse { get; set; }

        public ProfDataModel primaryProf { get; set; }
        public List<ProfDataModel> otherProfs { get; set; }

        /// <summary>
        /// Row is selected by user
        /// </summary>
        public bool IsSelected { get; set; }

        public decimal MeetingId { get; set; }
    }
}
