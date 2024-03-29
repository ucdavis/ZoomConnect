﻿using CanvasClient.Domain;
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
            if (assignmentRepo == null)
            {
                throw new ArgumentNullException("assignmentRepo");
            }

            if (courseRepo == null)
            {
                throw new ArgumentNullException("courseRepo");
            }

            var profAssignments = assignmentRepo.GetAll().Where(a => a.pidm == (prof.bannerPerson?.pidm ?? 0));
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
            if (prof?.zoomUser == null || String.IsNullOrEmpty(prof.zoomUser.id))
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
        public static CourseMeetingDataModel AddProfModels(this CourseMeetingDataModel meetingModel, List<ProfDataModel> profModels, CachedRepository<sirasgn> assignmentRepo, bool includePrimary = true)
        {
            assignmentRepo.GetAll()
                .Where(a => a.crn == meetingModel.bannerCourse.crn)
                .ToList()
                .ForEach(a =>
                {
                    var prof = profModels.FirstOrDefault(p => (p.bannerPerson?.pidm ?? 0) == a.pidm);
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
        /// Returns the Canvas Course sis_course_id for a Banner Course (CourseMeetingDataModel), as [TERM_CODE]-[SUBJ_CODE]-[CRSE_NUM]-[SEQ_NUM]-[CRN]
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public static string CanvasSisCourseId(this CourseMeetingDataModel meetingModel)
        {
            var ssbsect = meetingModel.bannerCourse;
            return $"{ssbsect.term_code}-{ssbsect.subj_code}-{ssbsect.crse_numb}-{ssbsect.seq_numb}-{ssbsect.crn}";
        }

        /// <summary>
        /// Create a list of Canvas CalendarEvent requests, one for each weekday of the course meeting.
        /// Canvas recurrence differs from Zoom so we have to do one for each weekday.
        /// </summary>
        /// <param name="meetingModel"></param>
        public static List<CalendarEventRequest> NewCanvasCalendarRequests(this CourseMeetingDataModel meetingModel,
            DateTime canvasStart, DateTime canvasEnd)
        {
            if (meetingModel == null || meetingModel.canvasCourse == null || meetingModel.bannerCourse == null)
            {
                return null;
            }

            var requests = new List<CalendarEventRequest>(meetingModel.DayNumbers(0).Count);
            meetingModel.WeekdayRecurrences(canvasStart, canvasEnd)
                .Where(wr => wr.startDateTime >= canvasStart && wr.startDateTime < canvasEnd)
                .ToList()
                .ForEach(wr =>
                {
                    requests.Add(new CalendarEventRequest
                    {
                        calendar_event = new EventRequestData
                        {
                            context_code = $"course_{meetingModel.canvasCourse.id}",
                            title = $"{meetingModel.bannerCourse.subj_code} {meetingModel.bannerCourse.crse_numb} Zoom",
                            start_at = wr.startDateTime,
                            end_at = wr.startDateTime.AddMinutes(meetingModel.DurationMinutes),
                            description = $"<a href='{meetingModel.zoomMeeting.join_url}'>Join with Zoom</a>",
                            duplicate = new EventRecurrence
                            {
                                count = wr.occurrences
                            }
                        }
                    });
                });

            return requests;
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
        /// Zoom formatted datetime in local time zone
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToZoomUTC(this DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
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
