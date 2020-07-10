﻿using System;
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
        private readonly SecretConfigManager<ZoomOptions> _secretOptions;

        public HomeController(ILogger<HomeController> logger, SecretConfigManager<ZoomOptions> secretOptions)
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

            // add another credential
            var countString = secrets.Creds.Count.ToString();
            secrets.Creds.Add(new GenericCredential { Username = $"bob{countString}", Password = $"secret{countString}!" });
            _secretOptions.Save();

            ViewBag.Secret = $"secret changing from '{oldsecret}' to '{secrets.Secret}'.";
            ViewBag.NotSecret = $"non-secret changing from '{oldplain}' to '{secrets.NotSecret}'.";
            ViewBag.CredsStored = countString;

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
