using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThijsTijsma.WritableConfiguration.Json;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWritableOptions<ZoomOptions> _zoomOptions;

        public HomeController(ILogger<HomeController> logger, IWritableOptions<ZoomOptions> zoomOptions)
        {
            _logger = logger;
            _zoomOptions = zoomOptions;
        }

        public IActionResult Index()
        {
            ViewBag.Secret = _zoomOptions.Value.Secret;
            _zoomOptions.Value.Secret += "!";
            _zoomOptions.Write();

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
