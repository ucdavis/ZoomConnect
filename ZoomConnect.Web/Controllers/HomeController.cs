using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using ZoomConnect.Web.Banner;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SecretConfigManager<ZoomOptions> _secretOptions;
        private const string _passwordPlaceholder = "*********";

        public HomeController(ILogger<HomeController> logger, SecretConfigManager<ZoomOptions> secretOptions)
        {
            _logger = logger;
            _secretOptions = secretOptions;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Setup()
        {
            var options = _secretOptions.GetValue().Result;
            var viewModel = new BannerOptionsViewModel
            {
                Instance = options.Banner.Instance,
                Username = options.Banner.Username.Value,
                Password = String.IsNullOrEmpty(options.Banner.Password.Value) ? "" : _passwordPlaceholder,

                CurrentTerm = options.CurrentTerm,
                CurrentSubject = options.CurrentSubject
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Setup(BannerOptionsViewModel model)
        {
            var options = _secretOptions.GetValue().Result;
            options.Banner.Instance = model.Instance;
            options.Banner.Username = new SecretStruct(model.Username);
            if (model.Password != _passwordPlaceholder && !String.IsNullOrEmpty(model.Password))
            {
                options.Banner.Password = new SecretStruct(model.Password);
            }

            options.CurrentTerm = model.CurrentTerm;
            options.CurrentSubject = model.CurrentSubject;

            _secretOptions.Save();

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Test([FromServices] SsbsectRepository ssbsect)
        {
            var success = ssbsect.TestConnection();
            ViewData["TestResult"] = success;

            if (success)
            {
                var rows = ssbsect.ReadCurrent();
                ViewData["RowCount"] = rows.Count;
            }

            return View();
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
