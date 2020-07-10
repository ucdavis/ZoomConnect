using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.SecretJsonConfig;

namespace ZoomConnect.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Secrets<ZoomOptions> _secretOptions;

        public HomeController(ILogger<HomeController> logger, Secrets<ZoomOptions> secretOptions)
        {
            _logger = logger;
            _secretOptions = secretOptions;
        }

        public IActionResult Index()
        {
            var secrets = _secretOptions.GetValue().Result;
            var oldsecret = secrets.Secret;
            var oldplain = secrets.NotSecret;
            secrets.Secret += " shh!";
            secrets.NotSecret += " hey!";
            ViewBag.Secret = $"secret changing from '{oldsecret}' to '{secrets.Secret}'.";
            ViewBag.NotSecret = $"non-secret changing from '{oldplain}' to '{secrets.NotSecret}'.";
            _secretOptions.Save();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
