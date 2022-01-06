using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretJsonConfig;
using ZoomClient;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Zoom;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    [Authorize("ConfiguredAdmins")]
    [TypeFilter(typeof(CheckRequirements))]
    public class StudentsController : Controller
    {
        private CachedStudentModels _studentFinder;
        private Zoom _zoomClient;

        public StudentsController(CachedStudentModels studentFinder, ZoomClient.Zoom zoomClient, SecretConfigManager<ZoomOptions> optionsManager)
        {
            _studentFinder = studentFinder;
            _zoomClient = zoomClient;

            var _options = optionsManager.GetValue().Result;
            _zoomClient.Options = _options.ZoomApi.CreateZoomOptions();
        }

        public IActionResult Index()
        {
            var studentModels = new SelectedStudentsModel
            {
                Students = _studentFinder.Students.Select(s => new StudentViewModel(s))
                    .OrderBy(m => m.Name)
                    .ToList()
            };

            return View(studentModels);
        }

        [HttpPost]
        public IActionResult Index(SelectedStudentsModel model)
        {
            var planUsage = _zoomClient.GetPlanUsage();
            model.RemainingLicenses = planUsage.plan_base.hosts - planUsage.plan_base.usage;

            model.Students = RehydrateSelectedStudents(model)
                .Select(s => new StudentViewModel(s) { IsSelected = true })
                .ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(SelectedStudentsModel model, [FromServices] ZoomUserCreator userCreator)
        {
            var students = RehydrateSelectedStudents(model);

            var created = userCreator.CreateLicensedZoomStudents(students);

            TempData["Message"] = $"{created.Count} Zoom User(s) licensed.";

            return RedirectToAction("Index");
        }

        public IActionResult Unlicense()
        {
            return View(new EmailListModel());
        }

        [HttpPost]
        public IActionResult Unlicense(EmailListModel emailListModel)
        {

            return View(emailListModel);
        }

        private List<StudentDataModel> RehydrateSelectedStudents(SelectedStudentsModel model)
        {
            var selectedPidms = model.Students
                .Where(m => m.IsSelected)
                .Select(m => m.Pidm);

            return _studentFinder.Students
                .Where(p => selectedPidms.Contains(p.bannerPerson.pidm))
                .OrderBy(p => p.bannerPerson.last_name)
                .ThenBy(p => p.bannerPerson.first_name)
                .ToList();
        }
    }
}
