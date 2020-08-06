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
            var termStart = _options.TermStart;
            var termEnd = _options.TermEnd;
            var meetingsFromCache = _cachedMeetings.Meetings;
            var random = new Random();

            var createdMeetings = new List<Meeting>(courseMeetings.Count);
            courseMeetings.ForEach(meeting =>
            {
                if (meeting.zoomMeeting != null) { return; }
                // TODO Look for meeting in Zoom api to make sure it is not already created.

                var requireAuth = _options.ZoomApi?.RequireMeetingAuthentication ?? false;

                var request = new MeetingRequest
                {
                    topic = meeting.MeetingName,
                    start_time = meeting.StartDateTime.ToZoomLocalTime(),
                    duration = meeting.DurationMinutes,
                    agenda = meeting.bannerMeeting.GetZoomMeetingAgenda(),
                    password = random.Next(100000, 999999).ToString(),
                    recurrence = new Recurrence
                    {
                        type = 2,
                        repeat_interval = 1,
                        weekly_days = String.Join(",", meeting.DayNumbers(1)),
                        end_times = (int)(termEnd.Subtract(termStart).TotalDays * meeting.DayNumbers(0).Count / 7)
                    },
                    settings = new MeetingSettings
                    {
                        alternative_hosts = _options.ZoomApi?.AlternateHosts,
                        meeting_authentication = requireAuth,
                        authentication_option = requireAuth ? _options.ZoomApi?.AuthenticationOptionId : null,
                        authentication_domains = requireAuth ? _options.ZoomApi?.AuthenticationDomains : null
                    }
                };

                var result = _zoomClient.CreateMeetingForUser(request, meeting.primaryProf.zoomUser.id);

                // TODO Erase meetings occurring on a holiday

                // stored created meeting result back in cached meeting list
                var foundInCache = meetingsFromCache.FirstOrDefault(cm => cm.bannerMeeting.GetZoomMeetingAgenda() == result.agenda);
                if (foundInCache != null)
                {
                    foundInCache.zoomMeeting = result;
                }
                createdMeetings.Add(result);
            });

            // save meetings back to cache
            _cachedMeetings.Set(meetingsFromCache);

            return createdMeetings;
        }
    }
}
