using CanvasClient;
using CanvasClient.Domain;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.Services.Canvas
{
    public class CalendarEventFinder
    {
        private CanvasApi _canvasApi;
        private List<Course> _courses;
        private ZoomOptions _options;

        public CalendarEventFinder(CanvasApi canvasApi, CachedCanvasCourses courses, SecretConfigManager<ZoomOptions> optionsManager)
        {
            _canvasApi = canvasApi;
            _courses = courses.Courses;
            _options = optionsManager.GetValue().Result;
        }

        /// <summary>
        /// Gets canvas calendar events for courses linked to Zoom and attaches them to the appropriate CourseMeetingDataModel.
        /// </summary>
        /// <param name="meetingModels"></param>
        /// <returns>List of CourseMeetingDataModels with found CalendarEvents attached</returns>
        public List<CourseMeetingDataModel> AttachEvents(List<CourseMeetingDataModel> meetingModels)
        {
            // get Calendar Events only for linked Zoom Meetings.
            meetingModels.Where(m => m.zoomMeeting != null)
                .ToList()
                .ForEach(m =>
                {
                    var course = _courses.FirstOrDefault(c => c.sis_course_id == m.CanvasSisCourseId());
                    if (course == null) { return; }
                    m.canvasCourse = course;
                    var events = _canvasApi.ListCalendarEvents(course.id, _options.TermStart, _options.TermEnd);
                    if (events == null) { return; }
                    m.canvasEvents = events
                        .Where(e => e.description.Contains(m.zoomMeeting.join_url, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                });

            return meetingModels;
        }
    }
}
