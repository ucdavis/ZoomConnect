using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ZoomClient.Domain;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Banner.Repository;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    public class ParticipantReportService
    {
        private CachedMeetingModels _meetingModels;
        private ZoomClient.Zoom _zoomClient;
        private SecretConfigManager<ZoomOptions> _configManager;
        private ZoomOptions _options;
        private GoremalRepository _goremalRepository;
        private SpridenRepository _spridenRepository;
        private ILogger<ParticipantReportService> _logger;

        public ParticipantReportService(CachedMeetingModels meetingModels, ZoomClient.Zoom zoomClient,
            SecretConfigManager<ZoomOptions> configManager, GoremalRepository goremalRepository,
            SpridenRepository spridenRepository, ILogger<ParticipantReportService> logger)
        {
            _meetingModels = meetingModels;
            _zoomClient = zoomClient;
            _configManager = configManager;
            _options = configManager.GetValue().Result;
            _goremalRepository = goremalRepository;
            _spridenRepository = spridenRepository;
            _logger = logger;

            _zoomClient.Options = _options.ZoomApi.CreateZoomOptions();
        }

        public List<ParticipantReportModel> PrepareReports()
        {
            // check last run time (default to today's meetings)
            var lastRunDate = _options?.LastParticipantReportDate ?? DateTime.Now.Date;
            var newRunDate = DateTime.Now;

            // get meetings with past instances after last run time
            List<ParticipantReportModel> reportModels = new List<ParticipantReportModel>();
            _meetingModels.Meetings
                .ToList()
                .ForEach(m =>
                {
                    if (String.IsNullOrEmpty(m.ZoomMeetingId)) { return; }

                    var pastInstances = _zoomClient.GetPastMeetingInstances(m.ZoomMeetingId);
                    if (pastInstances == null) { return; }

                    var newInstances = pastInstances
                        .Where(i => i.EndDateTime > lastRunDate)
                        .Select(i => new ParticipantReportModel
                        {
                            hostEmail = m.ProfEmail,
                            subject = $"{m.Subject} {m.CourseNum} {m.ProfLastName} - Attendance on {i.StartDateTimeLocal}",
                            instanceId = i.uuid,
                            crn = m.Crn
                        });
                    if (newInstances != null)
                    {
                        reportModels.AddRange(newInstances);
                    }
                });

            // prepare report bodies by getting participant report for each model created above
            reportModels.ForEach(rm =>
            {
                var participants = _zoomClient.GetParticipantReport(rm.instanceId);

                // list participants
                rm.participants = participants == null
                    ? new List<Participant>()
                    : participants
                        .OrderBy(pr => pr.name ?? "")
                        .ToList();

                // list non-participants (enrolled, not found by email)
                var participantEmails = rm.participants.Select(p => p.user_email).ToList();
                var enrolled = _goremalRepository.GetRegisteredStudents(rm.crn)
                    .Select(e => new spriden_student { pidm = e.pidm, email = e.email_address })
                    .ToList();
                rm.nonParticipants = enrolled
                    .Where(e => !participantEmails.Contains(e.email, StringComparer.OrdinalIgnoreCase))
                    .Select(e => new Participant
                    {
                        user_email = e.email,
                        name = _spridenRepository.GetOneStudent(e.pidm)?.FirstAndLastName
                    })
                    .ToList();
            });

            _options.LastParticipantReportDate = newRunDate;
            _configManager.Save();

            return reportModels;
        }
    }
}
