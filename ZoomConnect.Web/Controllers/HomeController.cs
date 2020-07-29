using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using System.Diagnostics;
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

        [TypeFilter(typeof(CheckRequirements))]
        public IActionResult Privacy()
        {
            return View();
        }

        [TypeFilter(typeof(CheckRequirements))]
        public IActionResult Test()
        {
            return View();
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
