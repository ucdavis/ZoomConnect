using CanvasClient.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private DateTime _termStart;
        private DateTime _termEnd;

        public CourseMeetingDataModel(DateTime termStart, DateTime termEnd)
        {
            otherProfs = new List<ProfDataModel>();
            canvasEvents = new List<CalendarEvent>();
            _termStart = termStart;
            _termEnd = termEnd;
        }

        public ssrmeet bannerMeeting { get; set; }
        public ssbsect bannerCourse { get; set; }

        public ProfDataModel primaryProf { get; set; }
        public List<ProfDataModel> otherProfs { get; set; }

        public Meeting zoomMeeting { get; set; }

        public Course canvasCourse { get; set; }
        /// <summary>
        /// Canvas Events for this course containing the zoomMeeting join url
        /// </summary>
        public List<CalendarEvent> canvasEvents { get; set; }
        /// <summary>
        /// Canvas Events for this course not containing the zoomMeeting join url
        /// </summary>
        public List<CalendarEvent> otherEvents { get; set; }

        // Zoom meeting creation helper properties below
        public string sunday { get { return bannerMeeting?.sun_day ?? ""; } }
        public string monday { get { return bannerMeeting?.mon_day ?? ""; } }
        public string tuesday { get { return bannerMeeting?.tue_day ?? ""; } }
        public string wednesday { get { return bannerMeeting?.wed_day ?? ""; } }
        public string thursday { get { return bannerMeeting?.thu_day ?? ""; } }
        public string friday { get { return bannerMeeting?.fri_day ?? ""; } }
        public string saturday { get { return bannerMeeting?.sat_day ?? ""; } }

        public string daysConcat
        {
            get
            {
                return this.sunday + this.monday + this.tuesday + this.wednesday + this.thursday + this.friday + this.saturday;
            }
        }

        /// <summary>
        /// List of numbered days of the week this meeting occurs (0-based on Sunday matching C# DayOfWeek)
        /// </summary>
        /// <param name="offset">Number to add to day of week.  0=C# style, 1=Zoom style</param>
        public List<int> DayNumbers(int offset)
        {
            var days = new List<int>();
            if (bannerMeeting?.sun_day != null) { days.Add(0 + offset); }
            if (bannerMeeting?.mon_day != null) { days.Add(1 + offset); }
            if (bannerMeeting?.tue_day != null) { days.Add(2 + offset); }
            if (bannerMeeting?.wed_day != null) { days.Add(3 + offset); }
            if (bannerMeeting?.thu_day != null) { days.Add(4 + offset); }
            if (bannerMeeting?.fri_day != null) { days.Add(5 + offset); }
            if (bannerMeeting?.sat_day != null) { days.Add(6 + offset); }

            return days;
        }

        /// <summary>
        /// List of weekdays for this course, with a start date and number of occurrences for each.
        /// </summary>
        /// <returns></returns>
        public List<WeekdayRecurrence> WeekdayRecurrences()
        {
            return DayNumbers(0)
                .Select(d => new WeekdayRecurrence(NextOccurrence, d, _termEnd))
                .ToList();
        }

        /// <summary>
        /// Start date and time of next occurrence.
        /// </summary>
        public DateTime StartDateTime
        {
            get
            {
                return NextOccurrence;
            }
        }

        /// <summary>
        /// End date and time of next occurrence
        /// </summary>
        public DateTime EndDateTime
        {
            get
            {
                return NextOccurrence.AddMinutes(DurationMinutes);
            }
        }

        /// <summary>
        /// Duration of meeting in minutes
        /// </summary>
        public int DurationMinutes
        {
            get
            {
                return EndMinutesTotal - StartMinutesTotal;
            }
        }

        /// <summary>
        /// Hour the meeting starts (0-23)
        /// </summary>
        public int StartHour
        {
            get
            {
                if (bannerMeeting == null || bannerMeeting.begin_time == null) { return 0; }
                return int.Parse(bannerMeeting.begin_time.Substring(0, 2));
            }
        }

        /// <summary>
        /// Minute the meeting starts (0-59)
        /// </summary>
        public int StartMinute
        {
            get
            {
                if (bannerMeeting == null || bannerMeeting.begin_time == null) { return 0; }
                return int.Parse(bannerMeeting.begin_time.Substring(2, 2));
            }
        }

        /// <summary>
        /// Total minutes since midnight to start of meeting
        /// </summary>
        public int StartMinutesTotal
        {
            get
            {
                return StartHour * 60 + StartMinute;
            }
        }

        /// <summary>
        /// Hour the meeting ends (0-23)
        /// </summary>
        public int EndHour
        {
            get
            {
                if (bannerMeeting == null || bannerMeeting.end_time == null) { return 0; }
                return int.Parse(bannerMeeting.end_time.Substring(0, 2));
            }
        }

        /// <summary>
        /// Minute the meeting ends (0-59)
        /// </summary>
        public int EndMinute
        {
            get
            {
                if (bannerMeeting == null || bannerMeeting.end_time == null) { return 0; }
                return int.Parse(bannerMeeting.end_time.Substring(2, 2));
            }
        }

        /// <summary>
        /// Total minutes since midnight to end of meeting
        /// </summary>
        public int EndMinutesTotal
        {
            get
            {
                return EndHour * 60 + EndMinute;
            }
        }

        /// <summary>
        /// Return DateTime of next occurrence of this meeting.
        /// Returns earliest start time after term start
        /// </summary>
        public DateTime NextOccurrence
        {
            get
            {
                // no days in meeting? no occurrence.
                if (DayNumbers(0).Count == 0) { return DateTime.MinValue; }

                // Start with start of term at appropriate time and add days as needed for first occurrence
                var first = new DateTime(_termStart.Year, _termStart.Month, _termStart.Day, StartHour, StartMinute, 0);
                var daysInFirstWeek = DayNumbers(0).Where(d => d >= (int)first.DayOfWeek);
                var offset = (daysInFirstWeek.Count() == 0)
                    ? 7 - (int)first.DayOfWeek + DayNumbers(0).Min()
                    : daysInFirstWeek.Min() - (int)first.DayOfWeek;

                return first.AddDays(offset);
            }
        }

        /// <summary>
        /// Produce a unique, readable string to serve as meeting topic.
        /// </summary>
        public String MeetingName
        {
            get
            {
                if (bannerMeeting == null || bannerCourse == null) { return "ssrmeet or ssbsect row missing."; }

                return String.Format("{0} {1} {2} {3} {4} {5} {6}-{7} in {8} {9}",
                        bannerCourse.subj_code, bannerCourse.crse_numb, bannerCourse.crn, bannerCourse.crse_title, primaryProf.bannerPerson.last_name,
                        this.daysConcat, bannerMeeting.begin_time, bannerMeeting.end_time, bannerMeeting.bldg_code, bannerMeeting.room_code);
            }
        }
    }
}
