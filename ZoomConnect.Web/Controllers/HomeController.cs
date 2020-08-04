using CanvasClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ZoomOptions _options;

        public HomeController(ILogger<HomeController> logger, SecretConfigManager<ZoomOptions> secretOptions)
        {
            _logger = logger;
            _options = secretOptions.GetValue().Result;
        }

        public IActionResult Index()
        {
            return View();
        }

        [TypeFilter(typeof(CheckRequirements))]
        public IActionResult Privacy()
        {
            return View();
        }

        [TypeFilter(typeof(CheckRequirements))]
        public IActionResult Test([FromServices] CanvasApi canvasApi)
        {
            //// list active courses
            //var canvasAccount = _options.CanvasApi.SelectedAccount;
            //var canvasTerm = _options.CanvasApi.EnrollmentTerm;
            //var courses = canvasApi.ListActiveCourses(canvasAccount, canvasTerm);
            //courses = courses.OrderBy(c => c.course_code)
            //    .ToList();

            // list calendar events
            var events = canvasApi.ListCalendarEvents(467774, new DateTime(2020, 8, 24), new DateTime(2020, 11, 25));
            return View(events);
        }

        public IActionResult Refresh([FromServices] SizedCache sizedCache)
        {
            sizedCache.ResetCache();

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [Route("login")]
        public async Task Login(string returnUrl)
        {
            var props = new AuthenticationProperties { RedirectUri = returnUrl };
            await HttpContext.ChallengeAsync("CAS", props);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
