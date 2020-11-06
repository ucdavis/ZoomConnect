using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretJsonConfig;
using ZoomClient;
using ZoomClient.Domain;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.Controllers
{
    [Authorize("ConfiguredAdmins")]
    public class ReportsController : Controller
    {
        private ZoomOptions _options;
        private Zoom _zoomClient;

        public ReportsController(SecretConfigManager<ZoomOptions> optionsManager, ZoomClient.Zoom zoomClient)
        {
            _options = optionsManager.GetValue().Result;
            _zoomClient = zoomClient;

            _zoomClient.Options = _options.ZoomApi.CreateZoomOptions();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AllCourses([FromServices] CachedMeetingModels meetingModels)
        {
            var courses = meetingModels.Courses
                .OrderBy(c => c.bannerCourse.subj_code)
                .ThenBy(c => c.bannerCourse.crse_numb)
                .ThenBy(c => c.bannerCourse.seq_numb)
                .ToList();
            var meetingsByCrn = meetingModels.Meetings.ToDictionary(m => m.Crn);

            var output = new StringBuilder(courses.Count * 200);
            output.Append("CRN, Description, Time, Location, Professor, Next Occurrence, Canvas\r\n");
            courses.ForEach(c =>
            {
                //output.Append($"{c.bannerCourse.crn}," {c.Description}, {c.TimeAndDays}, {c.Location}, {c.Prof}, {c.}, {}");
            });

            string csv = "Charlie, Chaplin, Chuckles";
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "AllCourses.csv");
        }

        public IActionResult CanvasStatus([FromServices] CachedMeetingModels meetingModels)
        {
            var courses = meetingModels.Courses
                .OrderBy(c => c.bannerCourse.subj_code)
                .ThenBy(c => c.bannerCourse.crse_numb)
                .ThenBy(c => c.bannerCourse.seq_numb)
                .ToList();

            var output = new StringBuilder(courses.Count * 200);
            output.Append("CRN,Description,Prof,Zoom,Canvas Status,Created Events,Other Events\r\n");
            courses.ForEach(c =>
            {
                // banner course
                output.AppendFormat("{0},{1} {2} ({3}),{4},",
                    c.bannerCourse.crn,
                    c.bannerCourse.subj_code, c.bannerCourse.crse_numb, c.bannerCourse.seq_numb,
                    c.primaryProf.bannerPerson?.last_name);
                // zoom?
                output.AppendFormat("{0},", c.zoomMeeting == null ? "-" : c.zoomMeeting.id);
                // canvas?
                output.AppendFormat("{0},{1},{2}\r\n",
                    c.canvasCourse?.workflow_state,
                    c.canvasCourse == null ? 0 : c.canvasEvents.Count(),
                    c.canvasCourse == null ? 0 : c.otherEvents.Count());
            });

            return File(new UTF8Encoding().GetBytes(output.ToString()), "text/csv", "CanvasStatus.csv");
        }

        public IActionResult UnconnectedMeetings([FromServices] CachedRepository<goremal> goremal)
        {
            var output = new StringBuilder();
            output.Append("Prof Email,Meeting Id,Meeting Name,Agenda\r\n");

            // get preferred prof emails
            var profEmails = goremal.GetAll()
                .Where(g => g.preferred_ind == "Y")
                .ToList();

            // get all meetings without ssrmeet.id in agenda (not connected)
            profEmails.ForEach(e =>
            {
                var meetings = _zoomClient.GetMeetingsForUser(e.email_address, "scheduled");
                if (meetings == null) { return; }

                meetings.Where(m => m.agenda == null || m.agenda.Length < 10 || m.agenda.Substring(0, 10) != "ssrmeet.id")
                    .ToList()
                    .ForEach(m =>
                    {
                        output.AppendFormat("{0},{1},{2},{3}\r\n", e.email_address, m.id, m.topic, m.agenda ?? "");
                    });
            });

            return File(new UTF8Encoding().GetBytes(output.ToString()), "text/csv", "UnconnectedMeetings.csv");
        }

        public IActionResult ZoomOccurrences([FromServices] CachedMeetingModels meetingModels)
        {
            var zoomMeetingDetails = new List<Meeting>();

            var output = new StringBuilder(meetingModels.Meetings.Count * 200);
            output.Append("Topic, Count, First, Last\r\n");

            // get details for each connected zoom meeting id
            foreach (var meeting in meetingModels.Meetings)
            {
                var details = _zoomClient.GetMeetingDetails(meeting.ZoomMeetingId);
                if (details.occurrences == null)
                {
                    details.occurrences = new List<MeetingOccurrence>();
                }
                var topic = details.topic;
                var count = details.occurrences.Count;
                var first = DateTime.MinValue;
                var last = DateTime.MaxValue;
                if (details.occurrences.Count > 0)
                {
                    first = details.occurrences.OrderBy(o => o.StartDateTimeLocal).FirstOrDefault().StartDateTimeLocal;
                    last = details.occurrences.OrderBy(o => o.StartDateTimeLocal).LastOrDefault().StartDateTimeLocal;
                }
                output.AppendFormat("\"{0}\",{1},{2},{3}\r\n", topic, count, first, last);
            }

            return File(new System.Text.UTF8Encoding().GetBytes(output.ToString()), "text/csv", "ZoomOccurrences.csv");
        }
    }
}
