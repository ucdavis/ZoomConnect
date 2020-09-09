using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediasiteUtil;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SecretJsonConfig;
using ZoomClient;
using ZoomClient.Extensions;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.DependencyInjection;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services;
using ZoomConnect.Web.Services.Mediasite;
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
        private DirectoryManager _directoryManager;

        public CmdController(ZoomClient.Zoom zoomClient, CachedRepository<goremal> goremal,
            CachedRepository<ssbsect> ssbsect, CachedRepository<ssrmeet> ssrmeet,
            ParticipantReportService participantService, EmailService emailService,
            SecretConfigManager<ZoomOptions> optionsManager, DirectoryManager directoryManager)
        {
            _zoomClient = zoomClient;
            _goremal = goremal;
            _ssbsect = ssbsect;
            _ssrmeet = ssrmeet;
            _participantService = participantService;
            _emailService = emailService;
            _zoomOptions = optionsManager.GetValue().Result;
            _directoryManager = directoryManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CloudDownload()
        {
            // fail if not configured
            if (String.IsNullOrEmpty(_directoryManager.DownloadDirectory)) { return Content("Download Directory not configured"); }

            // get all prof emails
            var profEmails = _goremal.GetAll()
                .Select(e => e.email_address.ToLower())
                .ToList();

            // get additional prof emails from Setup
            profEmails.AddRange((_zoomOptions.ExtraProfEmails ?? "")
                .ToLower()
                .Split(new[] { ";", ",", " " }, StringSplitOptions.RemoveEmptyEntries)
                .ToList());

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
                return meetingAgendas.ContainsKey(meetingDetail?.agenda ?? "");
            });

            // recording download
            var output = new StringBuilder(80 * profRecordings.Count);
            output.AppendFormat("\r\nRecordings will be saved to [{0}].\r\n", _directoryManager.DownloadDirectory);

            output.AppendLine("  MEETING_ID  TYPE SIZE     START            END    ACTION");
            foreach (var meeting in connectedRecordings)
            {
                var recordingFiles = meeting.recording_files; //.Where(r => r.file_type.ToUpper() == "MP4" || r.file_type.ToUpper() == "M4A");
                foreach (var recording in recordingFiles)
                {
                    output.AppendFormat(" {0, -12} {1, -4} {2, 8} {3:yyyy-MM-dd HH:mm}-{4:HH:mm}  ",
                        meeting.id, recording.file_type, recording.file_size.ShortFileSize(), recording.RecordingStartDateTime, recording.RecordingEndDateTime);

                    if (recording.status.ToUpper() != "COMPLETED")
                    {
                        output.AppendFormat("{0}, ignoring for now...", recording.status);
                    }
                    else
                    {
                        var filename = recording.GetLocalFileName(meeting) + "." + recording.file_type;
                        var fileFullName = recording.file_type.ToUpper() == "MP4"
                            ? Path.Combine(_directoryManager.DownloadDirectory, filename)
                            : Path.Combine(_directoryManager.DownloadNonMp4Directory, filename);
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
        public IActionResult MediasiteUpload([FromServices] SecretConfigManager<List<MediasiteJob>> jobsFile,
            [FromServices] MediasiteClient msClient, [FromServices] CachedMeetingModels meetingModels,
            [FromServices] SecretConfigManager<List<MediasiteJob>> mediasiteJobsFile)
        {
            var mediasiteOptions = _zoomOptions.MediasiteOptions;
            msClient.Config = mediasiteOptions.CreateMediasiteConfig();
            var lawMp4Template = msClient.GetTemplates().First(t => t.Id == mediasiteOptions.TemplateId);
            var cachedMeetings = meetingModels.Meetings;

            //
            // MONITOR OUTSTANDING JOB STATUS, CLEANUP OLD JOBS
            //

            var output = new StringBuilder();
            output.AppendLine("Checking pending jobs...");

            var ignorePendingJobSessionIds = new List<String>();
            var emails = new List<MimeMessage>();
            var jobRows = mediasiteJobsFile.GetValue().Result ?? new List<MediasiteJob>();
            jobRows = jobRows.Where(j => j.Status != "Failed" && j.Status != "Successful").ToList();
            jobRows.ForEach(j =>
            {
                // session id includes zoom meeting id plus year, month, day of recording.
                // this should be sufficient to group additional files related to the same teaching session
                // without getting recordings for the same class but on different days.
                var sessionId = j.FileName.SessionIdPrefix();
                output.AppendFormat("{0} ", sessionId);
                // update job status for each pending job
                var jobStatus = msClient.GetJob(j.JobId);
                j.Status = jobStatus == null ? "missing" : jobStatus.Status;
                j.Tries++;
                j.ModifiedDate = DateTime.Now;

                output.AppendFormat("{0} ", j.Status);

                if (j.Status == "Successful")
                {
                    if (j.ExternalId == null || j.ExternalId.Length < 9)
                    {
                        output.Append(" meeting id missing, no cleanup to do.");
                    }
                    else
                    {
                        Directory.GetFiles(_directoryManager.UploadDirectory, sessionId + "_*.MP4")
                            .ToList()
                            .ForEach(f =>
                            {
                                Directory.Move(f, Path.Combine(_directoryManager.UploadOutDirectory, Path.GetFileName(f)));
                                output.Append(".");
                            });

                        bool emailOk = false;
                        try
                        {
                            var parsedFilename = new PresentationFileName(j.FileName);
                            if (parsedFilename.isOk)
                            {
                                var mail = new MimeMessage();
                                mail.From.Add(MailboxAddress.Parse(_zoomOptions.EmailOptions.Username));
                                mail.To.Add(MailboxAddress.Parse(_zoomOptions.MediasiteOptions.ReportToEmail));
                                mail.ReplyTo.Add(MailboxAddress.Parse(_zoomOptions.MediasiteOptions.ReportReplyToEmail));
                                mail.Subject = "Video Edit";
                                mail.Body = new TextPart("html")
                                {
                                    Text = String.Format("<p>The following presentation is uploaded to Mediasite, please process according to procedures.</p><p>FOLDER: {0}<br/>PRESENTATION: {1}<br/>TIME: {2}</p>",
                                        parsedFilename.Folder, j.PresentationName, parsedFilename.CourseDateTime.Value.ToShortTimeString())
                                };
                                emailOk = true;
                                emails.Add(mail);
                            }
                        }
                        catch (Exception ex)
                        {
                            output.Append(ex.ToString());
                        }
                        output.AppendFormat(" meeting files moved to {0}; email {1} ", _directoryManager.UploadOutDirectory, emailOk ? "sent" : "failed");
                    }
                }
                else
                {
                    ignorePendingJobSessionIds.Add(sessionId);
                    output.Append("ignoring files for this job.");
                }

                output.AppendLine();
            });

            _emailService.Send(emails);
            mediasiteJobsFile.Save();

            output.AppendFormat("\r\nProcessing new files...\r\n");

            //
            // GET MP4s IN UPLOAD DIRECTORY
            //

            var dirInfo = new DirectoryInfo(_directoryManager.UploadDirectory);
            dirInfo
                .GetFiles("*.MP4")
                .ToList()
                .Where(f => f.Name.SessionIdPrefix() != "")
                .Where(f => !ignorePendingJobSessionIds.Contains(f.Name.SessionIdPrefix()))
                .GroupBy(f => f.Name.SessionIdPrefix(), f => f, (key, g) => new MeetingFileGroup
                {
                    // grouped by meeting key so we only process one file per meeting (the largest one)
                    meetingId = key.Substring(0, key.IndexOf("_")),
                    files = g.OrderByDescending(s => s.Length).ToList()
                })
                .ToList()
                .ForEach(m =>
                {
                    output.AppendFormat("{0, -12}", m.meetingId);

                    // find term and crn for meeting
                    var bannerMeeting = cachedMeetings.FirstOrDefault(cm => cm.ZoomMeetingId == m.meetingId);
                    if (bannerMeeting != null)
                    {
                        m.meetingModel = bannerMeeting;
                        m.termCrn = String.Format("{0}-{1}", bannerMeeting.Term, bannerMeeting.Crn);
                        output.AppendFormat("{0, -13}", m.termCrn);
                    }
                    else
                    {
                        output.Append("No banner meeting found.");
                    }

                    // find mediasite folder
                    if (!String.IsNullOrEmpty(m.termCrn))
                    {
                        var matchingFolders = msClient.FindFoldersStartingWith(m.termCrn, mediasiteOptions.RootFolder)
                            .OrderBy(f => f.Name.Length);
                        if (matchingFolders.Count() > 0)
                        {
                            m.msFolderId = matchingFolders.First().Id;
                            output.Append("MS Folder found. ");
                        }
                        else
                        {
                            // gotta create folder
                            m.msFolderId = msClient.CreateFolder(m.termCrn, mediasiteOptions.RootFolder).Id;
                            output.Append("MS Folder added. ");
                        }
                    }

                    // create presentation
                    if (!String.IsNullOrEmpty(m.msFolderId))
                    {
                        m.parsedFileName = new PresentationFileName(m.selectedFile.Name);

                        if (!m.parsedFileName.isOk)
                        {
                            output.Append("Bad filename format, not uploading. ");
                            m.presentation = null;
                        }
                        else
                        {
                            // get presentation name [PROF - LAW ### - TITLE_M/D/YYYY - Zoom]
                            var presentationName = String.Format("{0} - {1} {2} - {3}_{4:M/d/yyyy HH:mm} - Zoom",
                                m.meetingModel.ProfLastName, m.meetingModel.Subject,
                                m.meetingModel.CourseNum, m.meetingModel.CourseTitle,
                                m.parsedFileName.CourseDateTime.Value);

                            output.AppendFormat("{0, -60} .", presentationName.FirstChars(60));

                            if (msClient.FindPresentationsInFolder(m.msFolderId).Any(p => p.Title == presentationName))
                            {
                                output.AppendFormat(" Presentation already exists. ");
                                output.AppendFormat(m.selectedFile.RenameOrDeleteWithPrefix('D')
                                    ? "Renamed with 'D' prefix. "
                                    : "Deleted ('D' prefix already exists). ");
                            }
                            else
                            {
                                m.presentation = msClient.CreatePresentation(lawMp4Template, m.msFolderId, mediasiteOptions.PlayerId, presentationName, m.parsedFileName.CourseDateTime.Value);
                            }
                        }
                    }
                    else
                    {
                        output.Append("MS Folder not found or created. ");
                    }

                    if (m.presentation != null)
                    {
                        // upload media to presentation
                        var uploadResponse = msClient.UploadMediaFileWithResponse(m.presentation.Id, m.selectedFile.FullName, m.selectedFile.Length, m.parsedFileName.GuidFileName);

                        if (uploadResponse.success && (uploadResponse.InnerResponse == null || uploadResponse.InnerResponse.success))
                        {
                            // add job row to table
                            var jobid = uploadResponse.InnerResponse.content;
                            var newJobRow = new MediasiteJob
                            {
                                App = "ZoomConnect",
                                JobId = jobid,
                                ExternalId = m.meetingId,
                                FileName = m.selectedFile.Name,
                                PresentationName = m.presentation.Title,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now
                            };
                            var jobs = mediasiteJobsFile.GetValue().Result;
                            if (jobs == null)
                            {
                                jobs = new List<MediasiteJob>();
                            }
                            jobs.Add(newJobRow);
                            mediasiteJobsFile.Save();

                            output.AppendFormat("upload job id {0}", jobid);
                        }
                        else
                        {
                            var response = uploadResponse.success ? uploadResponse.InnerResponse : uploadResponse;
                            output.AppendFormat("upload failed [{0} {1} {2}] ", response.statusCode, response.statusDescription, response.errorMessage);
                            output.AppendFormat(m.selectedFile.RenameOrDeleteWithPrefix('X')
                                ? "renaming with 'X' prefix. "
                                : "deleting ('X' prefix already exists). ");
                        }
                    }
                    else
                    {
                        output.Append("Presentation not created.");
                    }

                    output.AppendLine();
                });

            return Content(output.ToString());
        }

        // participant report
        public IActionResult ParticipantReport()
        {
            var messages = new List<MimeMessage>();
            var ccList = _zoomOptions.EmailOptions?.ParticipantReportCcList ?? "";

            var ccAddresses = ccList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => MailboxAddress.Parse(e))
                .ToList();

            _participantService.PrepareReports()
                .Where(r => r.participants.Count > 5)
                .ToList()
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

        /// <summary>
        /// Uploads jpg profile photos found in zoom photo directory.
        /// Files must be named with student id e.g. "999999999.jpg"
        /// </summary>
        /// <returns></returns>
        public IActionResult ProfilePhotos([FromServices] CachedRepository<spriden_student> studentRepo)
        {
            if (_zoomOptions.ProfilePhotoDirectory == null || !Directory.Exists(_zoomOptions.ProfilePhotoDirectory))
            {
                return Content("Profile Photo Directory not configured. Check Setup page.");
            }

            // prep output dir
            var outDir = Path.Combine(_zoomOptions.ProfilePhotoDirectory, "out");
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            // prep a line of output per file in directory
            var outputDictionary = new Dictionary<string, string>();

            // get all jpg photos in directory
            var dirInfo = new DirectoryInfo(_zoomOptions.ProfilePhotoDirectory);
            var photoFiles = dirInfo.GetFiles("*.jpg").ToList();

            if (photoFiles.Count == 0) { return Content("No files to upload."); }

            // index person and email rows to find zoom account email from student id.
            var emailById = studentRepo.GetAll().ToDictionary(s => s.id);

            // push each photo up to zoom profile
            photoFiles.ForEach(f =>
            {
                var studentId = f.Name.Substring(0, f.Name.Length - 4);
                if (!emailById.ContainsKey(studentId))
                {
                    outputDictionary.Add(f.Name, "Not found by id");
                    return;
                }

                var success = _zoomClient.UploadProfilePicture(emailById[studentId].email, f.FullName);
                outputDictionary.Add(f.Name, success ? "Uploaded" : "Failed");
            });

            // move successful files
            outputDictionary.Where(o => o.Value == "Uploaded")
                .ToList()
                .ForEach(o =>
                {
                    var inFile = Path.Combine(dirInfo.FullName, o.Key);
                    var outFile = Path.Combine(outDir, o.Key);
                    if (System.IO.File.Exists(outFile))
                    {
                        System.IO.File.Delete(inFile);
                    }
                    else
                    {
                        Directory.Move(inFile, outFile);
                    }
                });

            // prep report
            var output = String.Join("\r\n", outputDictionary.Select(o => $"{o.Key}\t{o.Value}"));

            return Content(output);
        }
    }
}
