using System;
using System.Collections.Generic;
using System.IO;
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
        private ZoomOptions _options;

        public StudentsController(CachedStudentModels studentFinder, ZoomClient.Zoom zoomClient, SecretConfigManager<ZoomOptions> optionsManager)
        {
            _studentFinder = studentFinder;
            _zoomClient = zoomClient;

            _options = optionsManager.GetValue().Result;
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

        public IActionResult Photos(string classLevel)
        {
            var students = _studentFinder.Students.Select(s => new StudentViewModel(s))
                .OrderBy(m => m.Name)
                .ToList();

            if (!String.IsNullOrEmpty(classLevel))
            {
                students = students.Where(s => s.ClassLevel == classLevel).ToList();
            }

            var studentModels = new SelectedStudentsModel
            {
                Students = students
            };

            var hasPhotoDirectory = !String.IsNullOrEmpty(_options.ProfilePhotoDirectory) && Directory.Exists(_options.ProfilePhotoDirectory);
            studentModels.Students.ForEach(s =>
            {
                if (String.IsNullOrEmpty(s.Email)) { return; }
                var zoomUser = _zoomClient.GetUser(s.Email);
                s.ProfilePhotoUrl = zoomUser?.pic_url;
                if (hasPhotoDirectory)
                {
                    s.HasLocalPhoto = System.IO.File.Exists(Path.Combine(_options.ProfilePhotoDirectory, s.StudentId + ".jpg"));
                }
            });

            return View(studentModels);
        }

        [HttpPost]
        public IActionResult Photos(SelectedStudentsModel model)
        {
            model.Students = RehydrateSelectedStudents(model)
                .Select(s => new StudentViewModel(s) { IsSelected = true, HasLocalPhoto = true })
                .ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Upload(SelectedStudentsModel model, [FromServices] ZoomUserCreator userCreator)
        {
            var students = RehydrateSelectedStudents(model);

            var created = userCreator.UploadStudentPhotos(students);

            TempData["Message"] = $"{created.Count} Zoom Photo(s) uploaded.";

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
            return RehydrateSelectedStudents(model, false);
        }

        private List<StudentDataModel> RehydrateSelectedStudents(SelectedStudentsModel model, bool includePhotos)
        {
            var selectedPidms = model.Students
                .Where(m => m.IsSelected)
                .Select(m => m.Pidm);

            var studentList = _studentFinder.Students
                .Where(p => selectedPidms.Contains(p.bannerPerson.pidm))
                .OrderBy(p => p.bannerPerson.last_name)
                .ThenBy(p => p.bannerPerson.first_name)
                .ToList();

            if (includePhotos)
            {

            }

            return studentList;
        }
    }
}
