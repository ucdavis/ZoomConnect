using System;
using ZoomClient.Domain;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Models
{
    /// <summary>
    /// Represents one student enrolled in classes in the currently selected term/subj.
    /// Holds links to relevant rows in Banner and Zoom.
    /// </summary>
    public class StudentDataModel
    {
        public StudentDataModel()
        {
        }

        public spriden_student bannerPerson { get; set; }
        public User zoomUser { get; set; }
    }
}
