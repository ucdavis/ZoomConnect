using CanvasClient;
using CanvasClient.Domain;
using Microsoft.Extensions.Caching.Memory;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.Services.Canvas
{
    public class CachedCalendarEvents
    {
        private CanvasApi _canvasApi;
        private CachedMeetingModels _zoomMeetings;
        private List<Course> _courses;
        private ZoomOptions _options;
        private IMemoryCache _cache;
        private const string _cacheKeyCalendarEvents = "foundCalendarEvents";

        public CachedCalendarEvents(CanvasApi canvasApi, CachedMeetingModels zoomMeetings, CachedCanvasCourses courses, 
            SecretConfigManager<ZoomOptions> optionsManager, SizedCache cache)
        {
            _canvasApi = canvasApi;
            _zoomMeetings = zoomMeetings;
            _courses = courses.Courses;
            _options = optionsManager.GetValue().Result;
            _cache = cache.Cache;
        }

        public List<CalendarEvent> Events
        {
            get
            {
                // see if courses are cached
                if (_cache.TryGetValue(_cacheKeyCalendarEvents, out List<CalendarEvent> cacheEntry))
                {
                    return cacheEntry;
                }

                // otherwise get Calendar Events only for linked Zoom Meetings.
                List<CalendarEvent> calendarEvents = new List<CalendarEvent>();

                _zoomMeetings.Meetings
                    .Where(m => m.zoomMeeting != null)
                    .ToList()
                    .ForEach(m =>
                    {
                        var course = _courses.FirstOrDefault(c => c.sis_course_id == m.CanvasSisCourseId());
                        if (course == null) { return; }
                        var events = _canvasApi.ListCalendarEvents(course.id, _options.TermStart, _options.TermEnd);
                        if (events == null) { return; }
                        calendarEvents.AddRange(events);
                    });

                Set(calendarEvents);

                return calendarEvents;
            }
        }

        public void Set(List<CalendarEvent> updatedCacheEntry)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(updatedCacheEntry.Count)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            _cache.Set(_cacheKeyCalendarEvents, updatedCacheEntry, cacheEntryOptions);
        }
    }
}
