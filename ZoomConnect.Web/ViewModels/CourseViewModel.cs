using System;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.ViewModels
{
    public class CourseViewModel
    {
        public CourseViewModel()
        {
        }

        public CourseViewModel(CourseMeetingDataModel course)
        {
            var section = course.bannerCourse;
            var meeting = course.bannerMeeting;

            MeetingId = meeting.surrogate_id;
            Crn = meeting.crn;
            TimeAndDays = $"{meeting.begin_time}-{meeting.end_time} {course.daysConcat}";
            Location = $"{meeting.bldg_code} {meeting.room_code}";
            Description = $"{section.subj_code} {section.crse_numb} {section.seq_numb} {section.crse_title}";
            Prof = course.primaryProf.bannerPerson.last_name;
            ProfZoomStatus = course.primaryProf.ZoomStatus();
            if (ProfZoomStatus == ZoomUserStatus.Connected)
            {
                ProfStatusCssClass = "oi oi-check text-success";
            }
            else if (ProfZoomStatus == ZoomUserStatus.Pending)
            {
                ProfStatusCssClass = "oi oi-clock text-warning";
            }
            else    // missing
            {
                ProfStatusCssClass = "oi oi-x text-danger";
            }
            NextOccurrence = course.NextOccurrence;
            IsMeetingConnected = course.zoomMeeting != null;
        }

        /// <summary>
        /// Is row selected by user or not.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Banner Meeting Surrogate Id, pk for ssrmeet, also the id for this row in a display list.
        /// </summary>
        public decimal MeetingId { get; set; }

        /// <summary>
        /// Banner course section identifier
        /// </summary>
        public string Crn { get; set; }

        /// <summary>
        /// Start and End Time and concatenated Days of week
        /// </summary>
        public string TimeAndDays { get; set; }

        /// <summary>
        /// Building and Room
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Course Subject, Number, and Title
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Primary Prof Last Name
        /// </summary>
        public string Prof { get; set; }

        /// <summary>
        /// Status of prof connection to zoom user
        /// </summary>
        public ZoomUserStatus ProfZoomStatus { get; set; }

        /// <summary>
        /// CSS classes for bootstrap/openiconic markup appropriate for prof zoom status
        /// </summary>
        public string ProfStatusCssClass { get; set; }

        /// <summary>
        /// Next occurrence of this meeting
        /// </summary>
        public DateTime NextOccurrence { get; set; }

        /// <summary>
        /// Is this course meeting connected a Zoom Meeting?
        /// </summary>
        public bool IsMeetingConnected { get; set; }
    }
}
