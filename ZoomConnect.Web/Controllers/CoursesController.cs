using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretJsonConfig;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Canvas;
using ZoomConnect.Web.Services.Zoom;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    [Authorize("ConfiguredAdmins")]
    [TypeFilter(typeof(CheckRequirements))]
    public class CoursesController : Controller
    {
        private CachedMeetingModels _meetingModels;
        private ZoomOptions _options;

        public CoursesController(CachedMeetingModels meetingModels, SecretConfigManager<ZoomOptions> optionsManager)
        {
            _meetingModels = meetingModels;
            _options = optionsManager.GetValue().Result;
        }

        public IActionResult Index()
        {
            var coursesModel = new SelectedCoursesViewModel
            {
                Courses = _meetingModels.Courses
                    .Select(m => new CourseViewModel(m, _options.CanvasApi?.UseCanvas ?? false))
                    .OrderBy(m => m.Description)
                    .ToList(),
                IncludeCanvas = _options.CanvasApi?.UseCanvas ?? false
            };

            return View(coursesModel);
        }

        [HttpPost]
        public IActionResult Index(SelectedCoursesViewModel model)
        {
            model.Courses = RehydrateSelectedMeetings(model)
                .Select(m => new CourseViewModel(m, _options.CanvasApi?.UseCanvas ?? false) { IsSelected = true })
                .ToList();
            model.IncludeCanvas = _options.CanvasApi?.UseCanvas ?? false;

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(SelectedCoursesViewModel model, [FromServices] ZoomMeetingCreator meetingCreator,
            [FromServices] CanvasEventCreator eventCreator)
        {
            var meetings = RehydrateSelectedMeetings(model);

            var created = meetingCreator.CreateZoomMeetings(meetings);
            TempData["Message"] = $"{created.Count} Zoom Meeting(s) created.";

            if (_options.CanvasApi.UseCanvas)
            {
                var createdEvents = eventCreator.FindOrCreateCanvasEvents(meetings);
                TempData["Message"] += $" {createdEvents.Count} Canvas Calendar Event(s) created.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Modify(SelectedCoursesViewModel model)
        {
            model.Courses = RehydrateSelectedMeetings(model)
                .Select(m => new CourseViewModel(m, _options.CanvasApi?.UseCanvas ?? false) { IsSelected = true })
                .ToList();
            model.IncludeCanvas = _options.CanvasApi?.UseCanvas ?? false;

            return View(model);
        }

        [HttpPost]
        public IActionResult Modify(SelectedCoursesViewModel model, [FromServices] ZoomMeetingScheduleUpdater zoomUpdater,
            [FromServices] CanvasScheduledEventUpdater canvasUpdater)
        {
            // validate holiday dates
            if (model.HolidayStart.HasValue && !model.HolidayEnd.HasValue ||
                !model.HolidayStart.HasValue && model.HolidayEnd.HasValue ||
                model.HolidayStart.HasValue && model.HolidayEnd.Value < model.HolidayStart.Value)
            {
                TempData["Message"] = "Invalid Holiday Dates";
                return RedirectToAction("Index");
            }

            // validate day-of-week variance settings
            if (model.VarianceDate.HasValue && !model.VarianceDayOfWeek.HasValue ||
                !model.VarianceDate.HasValue && model.VarianceDayOfWeek.HasValue ||
                model.VarianceDate.HasValue && model.VarianceDate.Value.DayOfWeek == model.VarianceDayOfWeek.Value)
            {
                TempData["Message"] = "Invalid Variance Dates";
                return RedirectToAction("Index");
            }

            // return to index if nothing to do
            if (!model.HolidayEnd.HasValue && !model.VarianceDate.HasValue)
            {
                TempData["Message"] = "No Modifications Selected";
                return RedirectToAction("Index");
            }

            // carry out modifications
            var results = "";
            if (model.HolidayStart.HasValue)
            {
                results = zoomUpdater.DeleteHolidayMeetings(model.HolidayStart.Value.Date, model.HolidayEnd.Value.Date.AddDays(1));

                if (_options.CanvasApi.UseCanvas)
                {
                    results += canvasUpdater.DeleteHolidayEvents(model.HolidayStart.Value.Date, model.HolidayEnd.Value.Date.AddDays(1));
                }
            }

            ViewData["Holiday"] = model.HolidayStart.HasValue ? $"{model.HolidayStart.Value} - {model.HolidayEnd.Value}" : "";
            ViewData["Variance"] = model.VarianceDate.HasValue ? $"{model.VarianceDate.Value} is a {model.VarianceDayOfWeek.Value}" : "";
            ViewData["Results"] = results;

            return View(model);
        }

        private List<CourseMeetingDataModel> RehydrateSelectedMeetings(SelectedCoursesViewModel model)
        {
            var selectedMeetingIds = model.Courses
                .Where(c => c.IsSelected)
                .Select(c => c.MeetingId);

            return _meetingModels.Courses
                .Where(m => selectedMeetingIds.Contains(m.bannerMeeting.surrogate_id))
                .OrderBy(m => m.bannerCourse.subj_code)
                .ThenBy(m => m.bannerCourse.crse_numb)
                .ThenBy(m => m.bannerCourse.seq_numb)
                .ToList();
        }
    }
}
