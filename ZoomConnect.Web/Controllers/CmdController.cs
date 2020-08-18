using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SecretJsonConfig;
using ZoomClient;
using ZoomClient.Extensions;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Services;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.Controllers
{
    [CorsAllowAll]
    [TypeFilter(typeof(CmdKeyAuthorize))]
    public class CmdController : Controller
    {
        private Zoom _zoomClient;
        private CachedRepository<goremal> _goremal;
        private CachedRepository<ssbsect> _ssbsect;
        private CachedRepository<ssrmeet> _ssrmeet;
        private ParticipantReportService _participantService;
        private EmailService _emailService;
        private ZoomOptions _zoomOptions;

        public CmdController(ZoomClient.Zoom zoomClient, CachedRepository<goremal> goremal,
            CachedRepository<ssbsect> ssbsect, CachedRepository<ssrmeet> ssrmeet,
            ParticipantReportService participantService, EmailService emailService,
            SecretConfigManager<ZoomOptions> optionsManager)
        {
            _zoomClient = zoomClient;
            _goremal = goremal;
            _ssbsect = ssbsect;
            _ssrmeet = ssrmeet;
            _participantService = participantService;
            _emailService = emailService;
            _zoomOptions = optionsManager.GetValue().Result;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CloudDownload()
        {
            // fail if not configured
            if (String.IsNullOrEmpty(_zoomOptions.DownloadDirectory)) { return Content("Download Directory not configured"); }

            // get all prof emails
            var profEmails = _goremal.GetAll()
                .Select(e => e.email_address.ToLower())
                .ToList();

            // get all recordings for account
            var recordings = _zoomClient.GetCloudRecordingsForAccount();

            // filter recordings for found profs
            var profRecordings = recordings.Where(r => profEmails.Contains(r.host_email.ToLower()))
                .ToList();

            // filter recordings for connected meetings only
            var meetingAgendas = _ssrmeet.GetAll().ToDictionary(m => m.GetZoomMeetingAgenda());
            var connectedRecordings = profRecordings.Where(r =>
            {
                var meetingDetail = _zoomClient.GetMeetingDetails(r.id);
                return meetingAgendas.ContainsKey(meetingDetail.agenda);
            });

            // directory setup
            var downloadDirectory = _zoomOptions.DownloadDirectory;
            var nonMP4Directory = Path.Combine(downloadDirectory, "non_mp4");
            if (!Directory.Exists(nonMP4Directory))
            {
                Directory.CreateDirectory(nonMP4Directory);
            }

            // recording download
            var output = new StringBuilder(80 * profRecordings.Count);
            output.AppendFormat("\r\nRecordings will be saved to [{0}].\r\n", downloadDirectory);

            output.AppendLine(" MEETING_ID  TYPE SIZE     START            END    ACTION");
            foreach (var meeting in connectedRecordings)
            {
                var recordingFiles = meeting.recording_files; //.Where(r => r.file_type.ToUpper() == "MP4" || r.file_type.ToUpper() == "M4A");
                foreach (var recording in recordingFiles)
                {
                    output.AppendFormat(" {0, -11} {1, -4} {2, 8} {3:yyyy-MM-dd HH:mm}-{4:HH:mm}  ",
                        meeting.id, recording.file_type, recording.file_size.ShortFileSize(), recording.RecordingStartDateTime, recording.RecordingEndDateTime);

                    if (recording.status.ToUpper() != "COMPLETED")
                    {
                        output.AppendFormat("{0}, ignoring for now...", recording.status);
                    }
                    else
                    {
                        var filename = recording.GetLocalFileName(meeting) + "." + recording.file_type;
                        var fileFullName = recording.file_type.ToUpper() == "MP4"
                            ? Path.Combine(downloadDirectory, filename)
                            : Path.Combine(nonMP4Directory, filename);
                        var recordingUrl = recording.download_url.Substring(recording.download_url.IndexOf("/", 8));

                        output.Append("downloading ");

                        var response = _zoomClient.DownloadRecordingStream(recordingUrl, fileFullName);

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            output.AppendFormat("N - status={0}, not downloaded.", response.StatusCode);
                        }
                        else
                        {
                            output.Append("Y, trashing ");
                            var deleted = _zoomClient.DeleteRecording(recording.meeting_id, recording.id);
                            output.AppendFormat("{0} [{1}]", deleted ? "Y" : "N", filename);
                        }
                    }
                    output.AppendLine();
                }
            }

            return Content(output.ToString());
        }

        // mediasite upload

        // participant report
        public IActionResult ParticipantReport()
        {
            var messages = new List<MimeMessage>();
            var ccList = _zoomOptions.EmailOptions?.ParticipantReportCcList ?? "";

            var ccAddresses = ccList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => MailboxAddress.Parse(e))
                .ToList();

            _participantService.PrepareReports()
                .ForEach(r =>
                {
                    var msg = new MimeMessage();
                    msg.From.Add(MailboxAddress.Parse(_zoomOptions.EmailOptions.Username));
                    msg.To.Add(MailboxAddress.Parse(r.hostEmail));
                    msg.Cc.AddRange(ccAddresses);
                    msg.Subject = r.subject;
                    msg.Body = new TextPart("html")
                    {
                        Text = r.participants.HtmlReport()
                    };
                    messages.Add(msg);
                });

            _emailService.Send(messages);

            var subjects = messages.Select(m => m.Subject)
                .ToList();

            var subjectLines = String.Join(Environment.NewLine, subjects);

            return Content(subjectLines);
        }
    }
}
