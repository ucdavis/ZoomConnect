using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Canvas;

namespace ZoomConnect.Web.Services.Zoom
{
    /// <summary>
    /// Finds Zoom Meeting associated with each course meeting in Banner
    /// </summary>
    public class ZoomMeetingFinder
    {
        private ZoomClient.Zoom _zoomClient;
        private CachedProfModels _profModels;
        private CachedRepository<ssrmeet> _meetingRepo;
        private CachedRepository<ssbsect> _courseRepo;
        private CachedRepository<sirasgn> _assignmentRepo;
        private CalendarEventFinder _canvasEventFinder;
        private ZoomOptions _options;

        private List<CourseMeetingDataModel> _meetings { get; set; }

        public ZoomMeetingFinder(ZoomClient.Zoom zoomClient, CachedProfModels profModels,
            CachedRepository<ssrmeet> meetingRepo, CachedRepository<ssbsect> courseRepo,
            CachedRepository<sirasgn> assignmentRepo, CalendarEventFinder canvasEventFinder,
            SecretConfigManager<ZoomOptions> optionsManager)
        {
            _zoomClient = zoomClient;
            _profModels = profModels;
            _meetingRepo = meetingRepo;
            _courseRepo = courseRepo;
            _assignmentRepo = assignmentRepo;
            _canvasEventFinder = canvasEventFinder;
            _options = optionsManager.GetValue().Result;
        }

        /// <summary>
        /// Course Meetings linked where possible to found Zoom meetings by prof-zoomuser and ssrmeet id
        /// </summary>
        public List<CourseMeetingDataModel> Meetings
        {
            get
            {
                if (_meetings == null)
                {
                    Find();
                }

                return _meetings;
            }
        }

        /// <summary>
        /// Searches for Zoom Meeting by each prof-zoomuser and ssrmeet id
        /// </summary>
        private void Find()
        {
            _meetings = new List<CourseMeetingDataModel>();

            // First, get all Zoom Meetings for Found ProfDataModels.
            // No zoom meetings can be found without a found Prof<->Zoom User
            // Get "scheduled" meetings so Agenda will be included, which contains the ssrmeet id.
            var allFoundProfMeetings = new List<CourseMeetingDataModel>();
            _profModels.Profs
                .Where(p => p.zoomUser != null)
                .ToList()
                .ForEach(p =>
                {
                    var foundProfMeetings = _zoomClient.GetMeetingsForUser(p.zoomUser.id, "scheduled");
                    foundProfMeetings.ForEach(pm =>
                    {
                        allFoundProfMeetings.Add(new CourseMeetingDataModel(_options.TermStart)
                        {
                            zoomMeeting = pm,
                            primaryProf = p
                        });
                    });
                });

            // Find each ssrmeet by id in meetings list from above
            var foundSsrmeetRows = new List<ssrmeet>();
            _meetingRepo.GetAll().ForEach(m =>
            {
                var foundMeeting = allFoundProfMeetings.FirstOrDefault(pm => pm.zoomMeeting.agenda == m.GetZoomMeetingAgenda());
                if (foundMeeting != null)
                {
                    foundSsrmeetRows.Add(m);

                    // add ssrmeet and ssbsect rows to meeting model
                    foundMeeting.bannerMeeting = m;
                    foundMeeting.bannerCourse = _courseRepo.GetAll().FirstOrDefault(c => c.term_code == m.term_code && c.crn == m.crn);

                    // add other found profs to meeting model
                    foundMeeting.AddProfModels(_profModels, _assignmentRepo, false);

                    _meetings.Add(foundMeeting);
                }
            });

            // What's left in ssrmeet are not found in Zoom
            var missingMeetings =_meetingRepo.GetAll()
                .Where(m => !foundSsrmeetRows.Contains(m))
                .Select(m => new CourseMeetingDataModel(_options.TermStart)
                {
                    bannerMeeting = m,
                    bannerCourse = _courseRepo.GetAll().FirstOrDefault(c => c.term_code == m.term_code && c.crn == m.crn)
                })
                .ToList();

            // add primary and other profs to meeting model
            missingMeetings.ForEach(m => m.AddProfModels(_profModels, _assignmentRepo));

            _meetings.AddRange(missingMeetings);

            // attach Canvas CalendarEvents if using canvas
            if (_options.CanvasApi.UseCanvas)
            {
                _meetings = _canvasEventFinder.AttachEvents(_meetings);
            }
        }
    }
}
