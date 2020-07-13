using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using ZoomConnect.Web.Models;

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
            var oldsecret = secrets.Secret.Value;
            var oldplain = secrets.NotSecret;
            secrets.Secret = new SecureStruct(secrets.Secret.Value + " shh!");
            secrets.NotSecret += " hey!";

            // add another credential
            //var countString = secrets.Creds.Count.ToString();
            //secrets.Creds.Add(new GenericCredential { Username = $"bob{countString}", Password = (SecretString)$"secret{countString}!" });
            _secretOptions.Save();

            ViewBag.Secret = $"secret changing from '{oldsecret}' to '{secrets.Secret.Value}'.";
            ViewBag.NotSecret = $"non-secret changing from '{oldplain}' to '{secrets.NotSecret}'.";
            //ViewBag.CredsStored = countString;

            //var secretToString = new SecretString(countString);
            //var stringToSecret = "bob";

            //var newSecret = (SecretString)stringToSecret;
            //var newString = newSecret.ToString();
            //ViewBag.StringTest = newSecret;

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
