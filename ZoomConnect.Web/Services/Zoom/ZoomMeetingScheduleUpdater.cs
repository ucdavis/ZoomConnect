using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomClient.Domain;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Services.Zoom
{
    public class ZoomMeetingScheduleUpdater
    {
        private ZoomOptions _options;
        private ZoomClient.Zoom _zoomClient;
        private CachedMeetingModels _cachedMeetings;

        public ZoomMeetingScheduleUpdater(SecretConfigManager<ZoomOptions> optionsManager, ZoomClient.Zoom zoomClient,
            CachedMeetingModels cachedMeetings)
        {
            _options = optionsManager.GetValue().Result;
            _zoomClient = zoomClient;
            _cachedMeetings = cachedMeetings;

            _zoomClient.Options = _options.ZoomApi.CreateZoomOptions();
        }

        public string DeleteHolidayMeetings(DateTime holidayStart, DateTime holidayEnd)
        {
            string message = "";

            // select cached zoom meetings within the holiday date range
            _cachedMeetings.Meetings.ForEach(m =>
            {
                var meetingDetails = _zoomClient.GetMeetingDetails(m.ZoomMeetingId);
                meetingDetails.occurrences
                    .Where(o => o.status != "deleted" &&
                        o.StartDateTimeLocal >= holidayStart &&
                        o.StartDateTimeLocal < holidayEnd)
                    .ToList()
                    //.ForEach(d => _zoomClient.DeleteMeeting(m.ZoomMeetingId, d.occurrence_id));
                    .ForEach(d => message += $"{m.ZoomMeetingId}/{d.occurrence_id} at {d.StartDateTimeLocal}\r\n");
            });

            return message;
        }
    }
}
