using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SecretConfigManager<ZoomOptions> _secretOptions;

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
            var bannerOptions = _secretOptions.GetValue().Result.Banner;
            var viewModel = new BannerOptionsViewModel
            {
                Instance = bannerOptions.Instance,
                Username = bannerOptions.Username.Value,
                Password = bannerOptions.Password.Value
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Setup(BannerOptionsViewModel model)
        {
            var options = _secretOptions.GetValue().Result;
            var bannerOptions = options.Banner;
            bannerOptions.Instance = model.Instance;
            bannerOptions.Username = new SecretStruct(model.Username);
            bannerOptions.Password = new SecretStruct(model.Password);

            _secretOptions.Save();

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
