using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretJsonConfig;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    [Authorize]
    public class SetupController : Controller
    {
        private SecretConfigManager<ZoomOptions> _secretOptions;
        private const string _passwordPlaceholder = "*********";

        public SetupController(SecretConfigManager<ZoomOptions> secretOptions)
        {
            _secretOptions = secretOptions;
        }

        public IActionResult Index()
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

            return RedirectToAction("Index", "Home");
        }
    }
}
