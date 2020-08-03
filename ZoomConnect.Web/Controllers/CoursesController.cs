using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretJsonConfig;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Zoom;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(CheckRequirements))]
    public class CoursesController : Controller
    {
        private CachedMeetingModels _meetingModels;

        public CoursesController(CachedMeetingModels meetingModels)
        {
            _meetingModels = meetingModels;
        }

        public IActionResult Index([FromServices] SecretConfigManager<ZoomOptions> optionsManager)
        {
            var options = optionsManager.GetValue().Result;

            var coursesModel = new SelectedCoursesViewModel
            {
                Courses = _meetingModels.Meetings
                    .Select(m => new CourseViewModel(m))
                    .OrderBy(m => m.Description)
                    .ToList(),
                IncludeCanvas = options.CanvasApi?.UseCanvas ?? false
            };

            return View(coursesModel);
        }

        [HttpPost]
        public IActionResult Index(SelectedCoursesViewModel model)
        {
            model.Courses = RehydrateSelectedMeetings(model)
                .Select(m => new CourseViewModel(m) { IsSelected = true })
                .ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(SelectedCoursesViewModel model, [FromServices] ZoomMeetingCreator meetingCreator)
        {
            var meetings = RehydrateSelectedMeetings(model);

            var created = meetingCreator.CreateZoomMeetings(meetings);

            TempData["Message"] = $"{created.Count} Zoom Meeting(s) created.";

            return RedirectToAction("Index");
        }

        private List<CourseMeetingDataModel> RehydrateSelectedMeetings(SelectedCoursesViewModel model)
        {
            var selectedMeetingIds = model.Courses
                .Where(c => c.IsSelected)
                .Select(c => c.MeetingId);

            return _meetingModels.Meetings
                .Where(m => selectedMeetingIds.Contains(m.bannerMeeting.surrogate_id))
                .OrderBy(m => m.bannerCourse.subj_code)
                .ThenBy(m => m.bannerCourse.crse_numb)
                .ThenBy(m => m.bannerCourse.seq_numb)
                .ToList();
        }
    }
}
