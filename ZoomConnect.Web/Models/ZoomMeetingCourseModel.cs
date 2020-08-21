using System;

namespace ZoomConnect.Web.Models
{
    /// <summary>
    /// Represents one Zoom Meeting with its attched Banner Course
    /// </summary>
    public class ZoomMeetingCourseModel
    {
        public ZoomMeetingCourseModel()
        {
        }

        public string ZoomMeetingId { get; set; }
        public string Term { get; set; }
        public string Crn { get; set; }
        public string ProfLastName { get; set; }
        public string ProfEmail { get; set; }
        public string Subject { get; set; }
        public string CourseNum { get; set; }
        public string CourseTitle { get; set; }
    }
}
