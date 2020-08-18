using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    public class ParticipantReportService
    {
        private CachedMeetingModels _meetingModels;
        private ZoomClient.Zoom _zoomClient;
        private SecretConfigManager<ZoomOptions> _configManager;
        private ZoomOptions _options;
        private ILogger<ParticipantReportService> _logger;

        public ParticipantReportService(CachedMeetingModels meetingModels, ZoomClient.Zoom zoomClient,
            SecretConfigManager<ZoomOptions> configManager, ILogger<ParticipantReportService> logger)
        {
            _meetingModels = meetingModels;
            _zoomClient = zoomClient;
            _configManager = configManager;
            _options = configManager.GetValue().Result;
            _logger = logger;
        }

        public List<ParticipantReportModel> PrepareReports()
        {
            // check last run time (default to today's meetings)
            var lastRunDate = _options?.LastParticipantReportDate ?? DateTime.Now.Date;
            var meetings = _meetingModels.Meetings;

            // get meetings with past instances after last run time
            List<ParticipantReportModel> reportModels = new List<ParticipantReportModel>();
            meetings.Where(m => m.zoomMeeting != null)
                .ToList()
                .ForEach(m =>
                {
                    var newInstances = _zoomClient.GetPastMeetingInstances(m.zoomMeeting.id)
                        .Where(i => i.EndDateTime > lastRunDate)
                        .Select(i => new ParticipantReportModel
                        {
                            hostEmail = m.primaryProf.primaryEmail.email_address,
                            subject = $"{m.bannerCourse.subj_code} {m.bannerCourse.crse_numb} {m.primaryProf.bannerPerson.last_name} - Attendance on {i.StartDateTimeLocal}",
                            instanceId = i.uuid
                        });
                    if (newInstances != null)
                    {
                        reportModels.AddRange(newInstances);
                    }
                });

            _options.LastParticipantReportDate = DateTime.Now;
            _configManager.Save();

            // prepare report bodies by getting participant report for each model created above
            reportModels.ForEach(rm =>
            {
                rm.participants = _zoomClient.GetParticipantReport(rm.instanceId)
                    .OrderBy(pr => pr.name)
                    .ToList();
            });

            return reportModels;
        }
    }
}
