using System;
using System.Collections.Generic;
using System.Linq;
using ZoomClient.Domain;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.ViewModels;

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

        public static ZoomUserStatus ZoomStatus(this ProfDataModel prof)
        {
            if (prof.zoomUser == null || String.IsNullOrEmpty(prof.zoomUser.id))
            {
                return ZoomUserStatus.Missing;
            }
            else if (String.IsNullOrEmpty(prof.zoomUser.email))
            {
                return ZoomUserStatus.Pending;
            }
            else if (prof.zoomUser.type == PlanType.Basic)
            {
                return ZoomUserStatus.Basic;
            }
            else
            {
                return ZoomUserStatus.Connected;
            }
        }

        /// <summary>
        /// Add ProfDataModels of profs teaching this course, including primary prof by default.
        /// </summary>
        /// <param name="meetingModel"></param>
        /// <param name="profModels"></param>
        /// <param name="assignmentRepo"></param>
        /// <param name="includePrimary"></param>
        /// <returns></returns>
        public static CourseMeetingDataModel AddProfModels(this CourseMeetingDataModel meetingModel, CachedProfModels profModels, CachedRepository<sirasgn> assignmentRepo, bool includePrimary = true)
        {
            var allProfModels = profModels.Profs;

            assignmentRepo.GetAll()
                .Where(a => a.crn == meetingModel.bannerCourse.crn)
                .ToList()
                .ForEach(a =>
                {
                    var prof = allProfModels.FirstOrDefault(p => p.bannerPerson.pidm == a.pidm);
                    if (prof == null) { return; }

                    if (a.primary_ind == "Y" && includePrimary)
                    {
                        meetingModel.primaryProf = prof;
                    }
                    if (a.primary_ind != "Y")
                    {
                        meetingModel.otherProfs.Add(prof);
                    }
                });

            return meetingModel;
        }

        /// <summary>
        /// Zoom formatted datetime in local time zone
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToZoomLocalTime(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        /// <summary>
        /// Zoom meeting agenda string for creating or matching zoom meetings to ssrmeet rows.
        /// </summary>
        /// <param name="ssrmeetRow"></param>
        /// <returns></returns>
        public static string GetZoomMeetingAgenda(this ssrmeet ssrmeetRow)
        {
            return $"ssrmeet.id={ssrmeetRow.surrogate_id}";
        }
    }
}
