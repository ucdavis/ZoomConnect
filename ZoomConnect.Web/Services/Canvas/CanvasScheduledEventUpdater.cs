using CanvasClient;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
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

            // select cached zoom meetings within the holiday date range
            _cachedMeetings.Meetings.ForEach(m =>
            {
            });

            return message;
        }
    }
}
