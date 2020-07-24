using System;
using System.Collections.Generic;
using ZoomClient.Domain;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Models
{
    /// <summary>
    /// Represents one professor teaching classes in the currently selected term/subj.
    /// Holds links to relevant rows in Banner, Zoom, etc.
    /// </summary>
    public class ProfDataModel
    {
        public ProfDataModel()
        {
        }

        public spriden bannerPerson { get; set; }
        public goremal primaryEmail { get; set; }
        public List<goremal> otherEmails { get; set; }
        public List<sirasgn> assignments { get; set; }

        public User zoomUser { get; set; }
    }
}
