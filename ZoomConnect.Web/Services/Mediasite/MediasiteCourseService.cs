using MediasiteUtil;
using Org.BouncyCastle.Tls;
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
        private MediasiteClient _mediasiteClient;
        private ZoomOptions _options;

        public MediasiteCourseService(CachedRepository<ssrmeet> meetingRepo,
            CachedRepository<ssbsect> courseRepo, CachedRepository<sirasgn> assignmentRepo,
            CachedRepository<spriden> personRepo, CachedRepository<goremal> goremalRepo,
            MediasiteClient mediasiteClient,
            SecretConfigManager<ZoomOptions> optionsManager)
        {
            _meetingRepo = meetingRepo;
            _courseRepo = courseRepo;
            _assignmentRepo = assignmentRepo;
            _personRepo = personRepo;
            _goremalRepo = goremalRepo;
            _mediasiteClient = mediasiteClient;

            _options = optionsManager.GetValue().Result;
            _mediasiteClient.Config = _options.MediasiteOptions.CreateMediasiteConfig();
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

            var coursesFolder = _mediasiteClient.FindFolder(_options.MediasiteOptions.CoursesDirectory);
            var msFolders = _mediasiteClient.FindFolders(coursesFolder.Id);

            return _meetingRepo.GetAll()
                .Where(m => courses.ContainsKey(m.crn) && primaryAssignments.ContainsKey(m.crn))
                .Select(m => new MediasiteCourseModel
                {
                    bannerMeeting = m,
                    bannerCourse = courses[m.crn],
                    primaryProf = people[primaryAssignments[m.crn].pidm],
                    MediasiteFolderId = msFolders.FirstOrDefault(f => f.Name == $"{m.term_code}-{m.crn}")?.Id,
                    MediasiteFolderName = $"{m.term_code}-{m.crn}"
                })
                .ToList();
        }
    }
}
