using CanvasClient;
using CanvasClient.Domain;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.Services.Canvas
{
    public class CanvasEventCreator
    {
        private CanvasApi _canvasApi;
        private CachedMeetingModels _cachedMeetings;
        private CachedCanvasCourses _cachedCourses;
        private ZoomOptions _options;

        public CanvasEventCreator(CanvasApi canvasApi, CachedMeetingModels cachedMeetings, CachedCanvasCourses cachedCourses,
            SecretConfigManager<ZoomOptions> optionsManager)
        {
            _canvasApi = canvasApi;
            _cachedMeetings = cachedMeetings;
            _cachedCourses = cachedCourses;
            _options = optionsManager.GetValue().Result;
        }

        /// <summary>
        /// Finds or creates new Canvas Calendar Events for existing course-connected Zoom Meetings.
        /// Stores created canvas events back in cached meetings list.
        /// </summary>
        public List<CalendarEvent> FindOrCreateCanvasEvents(List<CourseMeetingDataModel> meetingModels)
        {
            var meetingsFromCache = _cachedMeetings.Meetings;
            var createdEvents = new List<CalendarEvent>();

            meetingModels.ForEach(m =>
            {
                // skip if no connected zoom meeting
                if (m.zoomMeeting == null) { return; }

                // skip if cached canvas calendar events exist
                if (m.canvasEvents != null && m.canvasEvents.Any()) { return; }

                // look in fresh canvas for calendar events and skip if found or error
                Course canvasCourse = m.canvasCourse;
                if (canvasCourse == null)
                {
                    canvasCourse = _cachedCourses.Courses.FirstOrDefault(c => c.sis_course_id == m.CanvasSisCourseId());
                }
                if (canvasCourse == null) { return; }
                m.canvasCourse = canvasCourse;

                var foundEvents = _canvasApi.ListCalendarEvents(canvasCourse.id, _options.TermStart, _options.TermEnd);
                if (foundEvents == null) { return; }        // api returning null indicates error, so skip this meeting
                foundEvents = foundEvents
                    .Where(e => e.description.Contains(m.zoomMeeting.join_url, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (foundEvents.Any()) { return; }          // events already found for this course, so skip

                // nothing found in calendar, create new calendar events for course
                m.NewCanvasCalendarRequests().ForEach(r =>
                {
                    var newEvent = _canvasApi.CreateCalendarEvent(r);
                    if (newEvent != null)
                    {
                        m.canvasEvents.Add(newEvent);
                        createdEvents.Add(newEvent);
                    }
                });

                // store created CalendarEvents back in cached meetings list
                var foundInCache = meetingsFromCache.FirstOrDefault(cm => cm.bannerMeeting.surrogate_id == m.bannerMeeting.surrogate_id);
                if (foundInCache != null)
                {
                    foundInCache.canvasEvents = m.canvasEvents;
                }
            });

            // save meetings back to cache
            _cachedMeetings.Set(meetingsFromCache);

            return createdEvents;
        }

    }
}
