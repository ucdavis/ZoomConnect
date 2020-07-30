using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomClient.Domain;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    public class ZoomMeetingCreator
    {
        private SecretConfigManager<ZoomOptions> _optionsManager;
        private ZoomOptions _options;
        private ZoomClient.Zoom _zoomClient;
        private CachedMeetingModels _cachedMeetings;

        public ZoomMeetingCreator(SecretConfigManager<ZoomOptions> optionsManager, ZoomClient.Zoom zoomClient, CachedMeetingModels cachedMeetings)
        {
            _optionsManager = optionsManager;
            _options = optionsManager.GetValue().Result;
            _zoomClient = zoomClient;
            _cachedMeetings = cachedMeetings;
        }

        public List<Meeting> CreateZoomMeetings(List<CourseMeetingDataModel> courseMeetings)
        {
            var sequenceStart = DateTime.Now > _options.TermStart ? DateTime.Now : _options.TermStart;
            var termEnd = _options.TermEnd;
            var meetingsFromCache = _cachedMeetings.Meetings;

            var createdMeetings = new List<Meeting>(courseMeetings.Count);
            foreach (var meeting in courseMeetings)
            {
                var request = new MeetingRequest
                {
                    topic = meeting.MeetingName,
                    start_time = meeting.StartDateTime.ToZoomLocalTime(),
                    duration = meeting.DurationMinutes,
                    agenda = meeting.bannerMeeting.GetZoomMeetingAgenda(),
                    recurrence = new Recurrence
                    {
                        type = 2,
                        repeat_interval = 1,
                        weekly_days = String.Join(",", meeting.DayNumbers(1)),
                        end_times = (int)(termEnd.Subtract(sequenceStart).TotalDays * meeting.DayNumbers(0).Count / 7)
                    }
                };

                var result = _zoomClient.CreateMeetingForUser(request, meeting.primaryProf.zoomUser.id);

                // stored created meeting result back in cached meeting list
                var foundInCache = meetingsFromCache.FirstOrDefault(cm => cm.bannerMeeting.GetZoomMeetingAgenda() == result.agenda);
                if (foundInCache != null)
                {
                    foundInCache.zoomMeeting = result;
                }
                createdMeetings.Add(result);
            }

            // save meetings back to cache
            _cachedMeetings.Set(meetingsFromCache);

            return createdMeetings;
        }
    }
}
