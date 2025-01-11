using SecretJsonConfig;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Mediasite
{
    public class MediasiteCourseService
    {
        private CachedRepository<ssrmeet> _meetingRepo;
        private CachedRepository<ssbsect> _courseRepo;
        private CachedRepository<sirasgn> _assignmentRepo;
        private CachedRepository<spriden> _personRepo;
        private CachedRepository<goremal> _goremalRepo;
        private ZoomOptions _options;

        public MediasiteCourseService(CachedRepository<ssrmeet> meetingRepo,
            CachedRepository<ssbsect> courseRepo, CachedRepository<sirasgn> assignmentRepo,
            CachedRepository<spriden> personRepo, CachedRepository<goremal> goremalRepo,
            SecretConfigManager<ZoomOptions> optionsManager)
        {
            _meetingRepo = meetingRepo;
            _courseRepo = courseRepo;
            _assignmentRepo = assignmentRepo;
            _personRepo = personRepo;
            _goremalRepo = goremalRepo;

            _options = optionsManager.GetValue().Result;
        }

        public List<MediasiteCourseModel> GetCourseMeetings()
        {
            var emails = _goremalRepo.GetAll()
                .Where(g => g.preferred_ind == "Y")
                .ToDictionary(g => g.pidm);
            var people = _personRepo.GetAll()
                .Select(p => new ProfDataModel
                {
                    bannerPerson = p,
                    primaryEmail = emails.ContainsKey(p.pidm) ? emails[p.pidm] : null
                })
                .ToDictionary(p => p.bannerPerson.pidm);
            var primaryAssignments = _assignmentRepo.GetAll()
                .Where(a => a.primary_ind == "Y")
                .ToDictionary(a => a.crn);
            var courses = _courseRepo.GetAll()
                .ToDictionary(c => c.crn);

            return _meetingRepo.GetAll()
                .Select(m => new MediasiteCourseModel
                {
                    bannerMeeting = m,
                    bannerCourse = courses[m.crn],
                    primaryProf = people[primaryAssignments[m.crn].pidm]
                })
                .ToList();
        }
    }
}
