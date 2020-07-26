using System;
using System.Linq;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    public static class ZoomServiceExtensions
    {
        public static ProfDataModel AddAssignments(this ProfDataModel prof, CachedRepository<sirasgn> assignmentRepo, CachedRepository<ssbsect> courseRepo)
        {
            if (prof.bannerPerson == null)
            {
                throw new ArgumentException("ProfDataModel has no bannerPerson.");
            }

            if (assignmentRepo == null)
            {
                throw new ArgumentNullException("assignmentRepo");
            }

            if (courseRepo == null)
            {
                throw new ArgumentNullException("courseRepo");
            }

            var profAssignments = assignmentRepo.GetAll().Where(a => a.pidm == prof.bannerPerson.pidm);
            var allCourses = courseRepo.GetAll();
            prof.assignments.AddRange(
                profAssignments.Join(
                    allCourses,
                    a => new { a.term_code, a.crn },
                    p => new { p.term_code, p.crn },
                    (a, p) => new AssignmentModel(a, p)));

            return prof;
        }
    }
}
