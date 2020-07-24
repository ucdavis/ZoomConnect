using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Zoom;
using ZoomConnect.Web.SetupRequirements;
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
                Instance = options.Banner?.Instance,
                Username = options.Banner?.Username.Value,
                Password = String.IsNullOrEmpty(options.Banner?.Password.Value) ? "" : _passwordPlaceholder,

                CurrentTerm = options.CurrentTerm,
                CurrentSubject = options.CurrentSubject,

                ZoomApiKey = options.ZoomApi?.ApiKey,
                ZoomApiSecret = options.ZoomApi?.ApiSecret
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Setup(BannerOptionsViewModel model, [FromServices] SizedCache sizedCache)
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

            if (model.ZoomApiKey != _passwordPlaceholder && !String.IsNullOrEmpty(model.ZoomApiKey))
            {
                options.ZoomApi.ApiKey = new SecretStruct(model.ZoomApiKey);
            }

            if (model.ZoomApiSecret != _passwordPlaceholder && !String.IsNullOrEmpty(model.ZoomApiSecret))
            {
                options.ZoomApi.ApiSecret = new SecretStruct(model.ZoomApiSecret);
            }

            _secretOptions.Save();
            sizedCache.ResetCache();

            return RedirectToAction("Index");
        }

        [Authorize]
        [TypeFilter(typeof(CheckRequirements))]
        public IActionResult Test([FromServices] CachedRepository<goremal> table, [FromServices] ZoomUserFinder userFinder)
        {
            var goremalRows = table.GetAll();
            ViewData["GoremalCount"] = goremalRows.Count;

            var zoomUsers = userFinder.Find();
            ViewData["ZoomUserCount"] = zoomUsers.Count;

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
