using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

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

        private List<CourseMeetingDataModel> _foundMeetings { get; set; }
        private List<CourseMeetingDataModel> _missingMeetings { get; set; }

        public ZoomMeetingFinder(ZoomClient.Zoom zoomClient, CachedProfModels profModels,
            CachedRepository<ssrmeet> meetingRepo, CachedRepository<ssbsect> courseRepo)
        {
            _zoomClient = zoomClient;
            _profModels = profModels;
            _meetingRepo = meetingRepo;
            _courseRepo = courseRepo;
        }

        /// <summary>
        /// Course Meetings linked to Zoom meetings by prof-zoomuser and ssrmeet id
        /// </summary>
        public List<CourseMeetingDataModel> FoundMeetings
        {
            get
            {
                if (_foundMeetings == null)
                {
                    Find();
                }

                return _foundMeetings;
            }
        }

        /// <summary>
        /// Course Meetings this term not matched in Zoom.
        /// </summary>
        public List<CourseMeetingDataModel> MissingMeetings
        {
            get
            {
                if (_missingMeetings == null)
                {
                    Find();
                }

                return _missingMeetings;
            }
        }

        /// <summary>
        /// Searches for Zoom Meeting by each prof-zoomuser and ssrmeet id
        /// </summary>
        private void Find()
        {
            _foundMeetings = new List<CourseMeetingDataModel>();
            _missingMeetings = new List<CourseMeetingDataModel>();

            // First, get all Zoom Meetings for Found ProfDataModels.
            // No zoom meetings can be found without a found Prof<->Zoom User
            // Get "live" meetings so Agenda will be included, which contains the ssrmeet id.
            var allFoundProfMeetings = new List<CourseMeetingDataModel>();
            _profModels.FoundProfs.ForEach(p =>
            {
                var foundProfMeetings = _zoomClient.GetMeetingsForUser(p.zoomUser.id, "live");
                foundProfMeetings.ForEach(pm =>
                {
                    allFoundProfMeetings.Add(new CourseMeetingDataModel
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
                var foundMeeting = allFoundProfMeetings.FirstOrDefault(pm => pm.zoomMeeting.agenda == $"ssrmeet.id={m.surrogate_id}");
                if (foundMeeting != null)
                {
                    foundSsrmeetRows.Add(m);
                    foundMeeting.bannerMeeting = m;
                    foundMeeting.bannerCourse = _courseRepo.GetAll().FirstOrDefault(c => c.term_code == m.term_code && c.crn == m.crn);
                    _foundMeetings.Add(foundMeeting);
                }
            });

            // What's left in ssrmeet are not found in Zoom
            _missingMeetings.AddRange(_meetingRepo.GetAll()
                .Where(m => !foundSsrmeetRows.Contains(m))
                .Select(m => new CourseMeetingDataModel
                {
                    bannerMeeting = m,
                    bannerCourse = _courseRepo.GetAll().FirstOrDefault(c => c.term_code == m.term_code && c.crn == m.crn)
                }));
        }
    }
}
