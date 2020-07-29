using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZoomConnect.Web.Filters;
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

        public IActionResult Index()
        {
            var coursesModel = new SelectedCoursesViewModel
            {
                Courses = _meetingModels.Meetings
                    .Select(m => new CourseViewModel(m))
                    .OrderBy(m => m.Description)
                    .ToList()
            };

            return View(coursesModel);
        }

        [HttpPost]
        public IActionResult Index(SelectedCoursesViewModel model)
        {
            var selectedMeetingIds = model.Courses
                .Where(c => c.IsSelected)
                .Select(c => c.MeetingId);

            model.Courses = _meetingModels.Meetings
                .Select(m => new CourseViewModel(m) { IsSelected = true })
                .Where(c => selectedMeetingIds.Contains(c.MeetingId))
                .OrderBy(c => c.Description)
                .ToList();

            return View(model);
        }
    }
}
