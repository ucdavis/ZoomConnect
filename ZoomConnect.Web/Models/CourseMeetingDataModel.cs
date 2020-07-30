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
    /// Translates
    /// </summary>
    public class CourseMeetingDataModel
    {
        private DateTime _termStart;

        public CourseMeetingDataModel(DateTime termStart)
        {
            otherProfs = new List<ProfDataModel>();
            _termStart = termStart;
        }

        public ssrmeet bannerMeeting { get; set; }
        public ssbsect bannerCourse { get; set; }

        public ProfDataModel primaryProf { get; set; }
        public List<ProfDataModel> otherProfs { get; set; }

        public Meeting zoomMeeting { get; set; }

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
                if (bannerMeeting == null) { return 0; }
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
                if (bannerMeeting == null) { return 0; }
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
                if (bannerMeeting == null) { return 0; }
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
                if (bannerMeeting == null) { return 0; }
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
        /// Returns earliest future start time after term start (next meeting day if start time is past).
        /// </summary>
        public DateTime NextOccurrence
        {
            get
            {
                // start tentatively with greater of today or term start at the appropriate time, and add days as needed
                var start = DateTime.Now > _termStart ? DateTime.Now : _termStart;
                var timePastAdjust = 0;
                var occurrence = new DateTime(start.Year, start.Month, start.Day, StartHour, StartMinute, 0);
                var nowInMinutes = start.Hour * 60 + start.Minute;

                // any more occurrences found this week?
                var todayDOW = (int)DateTime.Now.DayOfWeek;
                if (nowInMinutes >= StartMinutesTotal)
                {
                    timePastAdjust++;
                }
                var dayNumbers = this.DayNumbers(0);
                var futureDays = dayNumbers.Where(d => d >= todayDOW + timePastAdjust);

                var dayAdjust = 0;
                if (futureDays.Count() == 0)
                {
                    dayAdjust = 7 - todayDOW + dayNumbers.Min();
                }
                else
                {
                    dayAdjust += futureDays.Min() - todayDOW;
                }
                return occurrence.AddDays(dayAdjust);
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
