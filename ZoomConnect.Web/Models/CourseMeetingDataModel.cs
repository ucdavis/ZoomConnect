using System;
using System.Collections.Generic;
using ZoomClient.Domain;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Models
{
    public class CourseMeetingDataModel
    {
        public CourseMeetingDataModel()
        {
        }

        public ssrmeet bannerMeeting { get; set; }
        public ssbsect bannerCourse { get; set; }
        public spriden primaryProf { get; set; }
        public List<spriden> otherProfs { get; set; }
        // email?

        public Meeting zoomMeeting { get; set; }
    }
}
