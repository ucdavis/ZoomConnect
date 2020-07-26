using System;
using System.Collections.Generic;
using ZoomClient.Domain;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Models
{
    /// <summary>
    /// Represents one course meeting in the currently selected term/subj.
    /// Holds links to relevant rows in Banner, Zoom, etc.
    /// </summary>
    public class CourseMeetingDataModel
    {
        public CourseMeetingDataModel()
        {
            otherProfs = new List<ProfDataModel>();
        }

        public ssrmeet bannerMeeting { get; set; }
        public ssbsect bannerCourse { get; set; }

        public ProfDataModel primaryProf { get; set; }
        public List<ProfDataModel> otherProfs { get; set; }

        public Meeting zoomMeeting { get; set; }
    }
}
