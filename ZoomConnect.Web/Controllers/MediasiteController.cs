using MediasiteUtil;
using Microsoft.AspNetCore.Mvc;
using SecretJsonConfig;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Mediasite;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    public class MediasiteController : Controller
    {
        private ZoomOptions _options;
        private MediasiteCourseService _mediasiteCourseService;

        public MediasiteController(SecretConfigManager<ZoomOptions> optionsManager, MediasiteCourseService mediasiteCourseService)
        {
            _options = optionsManager.GetValue().Result;

            _mediasiteCourseService = mediasiteCourseService;
        }

        public IActionResult Index()
        {
            var coursesModel = new SelectedMediasiteModel
            {
                Courses = _mediasiteCourseService.GetCourseMeetings()
            };

            return View(coursesModel);
        }

        [HttpPost]
        public IActionResult Index(SelectedMediasiteModel model)
        {
            model.Courses = RehydrateSelectedMeetings(model);

            return View(model);
        }

        //[HttpPost]
        //public IActionResult Create(SelectedCoursesViewModel model, [FromServices] MediasiteCourseCreator mediasiteService)
        //{
        //    var meetings = RehydrateSelectedMeetings(model);

        //    // do not limit zoom start/end dates, that is only for Canvas.
        //    var created = mediasiteService.CreateMediasiteCourses(meetings);
        //    TempData["Message"] = $"{created.Count} Zoom Meeting(s) created.";

        //    return RedirectToAction("Index");
        //}

        private List<MediasiteCourseModel> RehydrateSelectedMeetings(SelectedMediasiteModel model)
        {
            var selectedMeetingIds = model.Courses
                .Where(c => c != null && c.IsSelected)
                .Select(c => c.MeetingId)
                .ToList();

            return _mediasiteCourseService.GetCourseMeetings()
                .Where(m => selectedMeetingIds.Contains(m.bannerMeeting.surrogate_id))
                .OrderBy(m => m.bannerCourse.subj_code)
                .ThenBy(m => m.bannerCourse.crse_numb)
                .ThenBy(m => m.bannerCourse.seq_numb)
                .ToList();
        }
    }
}
