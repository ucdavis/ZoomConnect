﻿using CanvasClient;
using CanvasClient.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services;
using ZoomConnect.Web.Services.Canvas;
using ZoomConnect.Web.Services.Zoom;

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
        public IActionResult Test([FromServices] CachedMeetingModels meetingModels)
        {
            // all zoom meetings tied to Banner... should have multiple zoom meetings per ssrmeet in some cases
            var models = meetingModels.Meetings
                .OrderBy(m => m.Subject)
                .ThenBy(m => m.CourseNum)
                .ToList();

            return View(models);
        }

        [Authorize("ConfiguredAdmins")]
        public IActionResult Refresh([FromServices] SizedCache sizedCache, [FromServices] ILogger<HomeController> logger)
        {
            sizedCache.ResetCache();
            logger.LogInformation("Dumping cache by request");

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
