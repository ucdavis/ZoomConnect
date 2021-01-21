using CanvasClient;
using SecretJsonConfig;
using System;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.Services.Canvas
{
    public class CanvasScheduledEventUpdater
    {
        private ZoomOptions _options;
        private CanvasApi _canvasClient;
        private CachedMeetingModels _cachedMeetings;

        public CanvasScheduledEventUpdater(SecretConfigManager<ZoomOptions> optionsManager, CanvasApi canvasClient,
            CachedMeetingModels cachedMeetings)
        {
            _options = optionsManager.GetValue().Result;
            _canvasClient = canvasClient;
            _cachedMeetings = cachedMeetings;
        }

        public string DeleteHolidayEvents(DateTime holidayStart, DateTime holidayEnd)
        {
            string message = "";

            // select cached canvas events within the holiday date range, for each cached Course
            _cachedMeetings.Courses.ForEach(m =>
            {
                // skip if no connected zoom meeting or canvas course
                if (m.zoomMeeting == null) { return; }

                // skip if no canvas calendar events exist
                if (m.canvasEvents == null) { return; }

                // delete connected events within requested date range
                m.canvasEvents.Where(e => e.start_at >= holidayStart && e.start_at < holidayEnd)
                    .ToList()
                    .ForEach(e => message += $"{m.zoomMeeting.id} {e.start_at} {e.description}");
            });

            return message;
        }
    }
}
