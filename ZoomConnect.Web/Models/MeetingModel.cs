using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoomClient.Domain;
using ZoomClient.Extensions;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Models
{
    /// <summary>
    /// Model for joining SSRMEET banner row to related banner and zoom data.
    /// This is the primary data for Zoom interface.
    /// </summary>
    public class MeetingModel
    {
        public MeetingModel()
        {
        }

        /// <summary>
        /// Constructor for loading MeetingModel with data from SSRMEET row
        /// </summary>
        /// <param name="row"></param>
        public MeetingModel(ssrmeet row)
        {
            term = row.term_code;
            crn = row.crn;
            beginTime = row.begin_time;
            endTime = row.end_time;
            building = row.bldg_code;
            room = row.room_code;
            termStart = row.start_date;
            termEnd = row.end_date;
            category = row.catagory ?? "";
            sunday = row.sun_day ?? "";
            monday = row.mon_day ?? "";
            tuesday = row.tue_day ?? "";
            wednesday = row.wed_day ?? "";
            thursday = row.thu_day ?? "";
            friday = row.fri_day ?? "";
            saturday = row.sat_day ?? "";
            schedCode = row.schd_code ?? "";
            over_ride = row.over_ride ?? "";
            meetingNo = row.meet_no;
            hrsWeek = row.hrs_week;
            surrogate_id = row.surrogate_id;

            // TODO fix all missing references below for ssrmeet -> ssbsect and -> sirasgn
            //// course subject, number, title
            //subject = row.subj_code;
            //courseNum = row.crse_numb;
            //courseTitle = row.crse_title;

            //// prof name (primary preferred)
            //var sirasgnRows = (LawnetDataSet.sirasgnRow[])row.GetParentRows("ssrmeet_sirasgn");
            //var primaryRows = sirasgnRows.Where(s => !s.Issirasgn_primary_indNull() && s.sirasgn_primary_ind == "Y").ToList();
            //if (primaryRows.Any())
            //{
            //    var profSpriden = primaryRows.First().spridenRow;
            //    profLastName = profSpriden == null ? "" : profSpriden.SPRIDEN_LAST_NAME;
            //}
            //else if (sirasgnRows.Length > 0)
            //{
            //    profLastName = sirasgnRows[0].spridenRow.SPRIDEN_LAST_NAME;
            //}
            //else
            //{
            //    profLastName = "";
            //}
        }

        // ssrmeet columns
        public string term { get; set; }
        public string crn { get; set; }
        public string beginTime { get; set; }
        public string endTime { get; set; }
        public string building { get; set; }
        public string room { get; set; }
        public DateTime termStart { get; set; }
        public DateTime termEnd { get; set; }
        public string category { get; set; }
        public string sunday { get; set; }
        public string monday { get; set; }
        public string tuesday { get; set; }
        public string wednesday { get; set; }
        public string thursday { get; set; }
        public string friday { get; set; }
        public string saturday { get; set; }
        public string schedCode { get; set; }
        public string over_ride { get; set; }
        public decimal meetingNo { get; set; }
        public decimal hrsWeek { get; set; }
        public decimal surrogate_id { get; set; }

        // related data from banner
        public string subject { get; set; }
        public string courseNum { get; set; }
        public string courseTitle { get; set; }
        public string profLastName { get; set; }

        // related data from zoom
        public Meeting zoomMeeting { get; set; }
        public string zoomRoomUserId { get; set; }

        // properties
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
            if (!String.IsNullOrEmpty(this.sunday)) { days.Add(0 + offset); }
            if (!String.IsNullOrEmpty(this.monday)) { days.Add(1 + offset); }
            if (!String.IsNullOrEmpty(this.tuesday)) { days.Add(2 + offset); }
            if (!String.IsNullOrEmpty(this.wednesday)) { days.Add(3 + offset); }
            if (!String.IsNullOrEmpty(this.thursday)) { days.Add(4 + offset); }
            if (!String.IsNullOrEmpty(this.friday)) { days.Add(5 + offset); }
            if (!String.IsNullOrEmpty(this.saturday)) { days.Add(6 + offset); }

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
                return int.Parse(this.beginTime.Substring(0, 2));
            }
        }

        /// <summary>
        /// Minute the meeting starts (0-59)
        /// </summary>
        public int StartMinute
        {
            get
            {
                return int.Parse(this.beginTime.Substring(2, 2));
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
                return int.Parse(this.endTime.Substring(0, 2));
            }
        }

        /// <summary>
        /// Minute the meeting ends (0-59)
        /// </summary>
        public int EndMinute
        {
            get
            {
                return int.Parse(this.endTime.Substring(2, 2));
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
        /// Return DateTime of next occurrence of this meeting.  Returns earliest future start time (next week if start time is past).
        /// </summary>
        public DateTime NextOccurrence
        {
            get
            {
                // start tentatively with today's date at the appropriate time, and add days as needed
                var now = DateTime.Now;
                var timePastAdjust = 0;
                var occurrence = new DateTime(now.Year, now.Month, now.Day, StartHour, StartMinute, 0);

                // any more occurrences found this week?
                var todayDOW = (int)DateTime.Now.DayOfWeek;
                if (now.TotalMinutes() >= StartMinutesTotal)
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
                return String.Format("{0} {1} {2} {3} {4} {5} {6}-{7} in {8} {9}",
                        this.subject, this.courseNum, this.crn, this.courseTitle, this.profLastName,
                        this.daysConcat, this.beginTime, this.endTime, this.building, this.room);
            }
        }
    }
}
