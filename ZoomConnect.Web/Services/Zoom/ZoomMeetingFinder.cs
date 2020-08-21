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

        private List<CourseMeetingDataModel> _courseMeetings { get; set; }
        private List<ZoomMeetingCourseModel> _zoomMeetings { get; set; }

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
        /// Course Meetings linked where possible to primary found Zoom meeting by prof-zoomuser and ssrmeet id
        /// </summary>
        public List<CourseMeetingDataModel> Courses
        {
            get
            {
                if (_courseMeetings == null)
                {
                    Find();
                }

                return _courseMeetings;
            }
        }

        /// <summary>
        /// Course Meetings linked where possible to all found Zoom meetings by prof-zoomuser and ssrmeet id
        /// </summary>
        public List<ZoomMeetingCourseModel> Meetings
        {
            get
            {
                if (_zoomMeetings == null)
                {
                    Find();
                }

                return _zoomMeetings;
            }
        }

        /// <summary>
        /// Searches for Zoom Meeting by each prof-zoomuser and ssrmeet id
        /// </summary>
        private void Find()
        {
            _courseMeetings = new List<CourseMeetingDataModel>();
            _zoomMeetings = new List<ZoomMeetingCourseModel>();

            var ssrmeetByAgenda = _meetingRepo.GetAll()
                .ToDictionary(m => m.GetZoomMeetingAgenda());
            var ssbsectByCrn = _courseRepo.GetAll()
                .ToDictionary(c => c.crn);

            // First, get all Zoom Meetings for Found ProfDataModels.
            // No zoom meetings can be found without a found Prof<->Zoom User
            // Get "scheduled" meetings so Agenda will be included, which contains the ssrmeet id.
            var allFoundProfMeetings = new List<CourseMeetingDataModel>();
            var cachedProfs = _profModels.Profs;

            cachedProfs
                .Where(p => p.zoomUser != null)
                .ToList()
                .ForEach(p =>
                {
                    var foundProfMeetings = _zoomClient.GetMeetingsForUser(p.zoomUser.id, "scheduled");
                    foundProfMeetings.ForEach(pm =>
                    {
                        allFoundProfMeetings.Add(new CourseMeetingDataModel(_options.TermStart, _options.TermEnd)
                        {
                            zoomMeeting = pm,
                            primaryProf = p
                        });

                        if (pm.agenda != null && ssrmeetByAgenda.ContainsKey(pm.agenda))
                        {
                            var mtg = ssrmeetByAgenda[pm.agenda];
                            var sect = ssbsectByCrn[mtg.crn];
                            _zoomMeetings.Add(new ZoomMeetingCourseModel
                            {
                                ZoomMeetingId = pm.id,
                                ProfLastName = p.bannerPerson.last_name,
                                ProfEmail = p.primaryEmail.email_address,
                                Term = mtg.term_code,
                                Crn = mtg.crn,
                                Subject = sect.subj_code,
                                CourseNum = sect.crse_numb,
                                CourseTitle = sect.crse_title
                            });
                        }
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
                    foundMeeting.AddProfModels(cachedProfs, _assignmentRepo, false);

                    _courseMeetings.Add(foundMeeting);
                }
            });

            // What's left in ssrmeet are not found in Zoom
            var missingMeetings =_meetingRepo.GetAll()
                .Where(m => !foundSsrmeetRows.Contains(m))
                .Select(m => new CourseMeetingDataModel(_options.TermStart, _options.TermEnd)
                {
                    bannerMeeting = m,
                    bannerCourse = _courseRepo.GetAll().FirstOrDefault(c => c.term_code == m.term_code && c.crn == m.crn)
                })
                .ToList();

            // add primary and other profs to meeting model
            missingMeetings.ForEach(m => m.AddProfModels(cachedProfs, _assignmentRepo));

            _courseMeetings.AddRange(missingMeetings);

            // attach Canvas CalendarEvents if using canvas
            if (_options.CanvasApi.UseCanvas)
            {
                _courseMeetings = _canvasEventFinder.AttachEvents(_courseMeetings);
            }
        }
    }
}
