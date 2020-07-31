﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanvasClient;
using CanvasClient.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretJsonConfig;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    [Authorize]
    public class SetupController : Controller
    {
        private SecretConfigManager<ZoomOptions> _secretOptions;
        private CachedRepository<stvterm> _termRepository;
        private const string _passwordPlaceholder = "*********";

        public SetupController(SecretConfigManager<ZoomOptions> secretOptions, CachedRepository<stvterm> termRepository)
        {
            _secretOptions = secretOptions;
            _termRepository = termRepository;
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

                TermStart = options.TermStart,
                TermEnd = options.TermEnd,

                ZoomApiKey = options.ZoomApi?.ApiKey,
                ZoomApiSecret = options.ZoomApi?.ApiSecret,

                UseCanvas = options.CanvasApi.UseCanvas,
                CanvasAccessToken = options.CanvasApi.ApiAccessToken
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(BannerOptionsViewModel model, [FromServices] SizedCache sizedCache, [FromServices] CanvasApi canvasApi)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var options = _secretOptions.GetValue().Result;
            options.Banner.Instance = model.Instance;
            options.Banner.Username = new SecretStruct(model.Username);
            if (model.Password != _passwordPlaceholder && !String.IsNullOrEmpty(model.Password))
            {
                options.Banner.Password = new SecretStruct(model.Password);
            }

            // process term changes - new start/end dates. otherwise allow preferred start/end.
            if (options.CurrentTerm != model.CurrentTerm)
            {
                options.CurrentTerm = model.CurrentTerm;

                // look up new start/end dates
                var term = _termRepository.GetAll().FirstOrDefault(t => t.code == model.CurrentTerm);
                options.TermStart = term.start_date;
                options.TermEnd = term.end_date;

                // find term in Canvas and store its id in hidden field
                var bannerTerm = _termRepository.GetAll().FirstOrDefault(t => t.code == options.CurrentTerm);
                var canvasTerm = canvasApi.ListEnrollmentTerms().FirstOrDefault(t => t.MatchesBannerTermDesc(bannerTerm.description));
                options.CanvasApi.EnrollmentTerm = canvasTerm.id;
            }
            else
            {
                options.TermStart = model.TermStart;
                options.TermEnd = model.TermEnd;
            }

            options.CurrentSubject = model.CurrentSubject;

            if (model.ZoomApiKey != _passwordPlaceholder && !String.IsNullOrEmpty(model.ZoomApiKey))
            {
                options.ZoomApi.ApiKey = new SecretStruct(model.ZoomApiKey);
            }

            if (model.ZoomApiSecret != _passwordPlaceholder && !String.IsNullOrEmpty(model.ZoomApiSecret))
            {
                options.ZoomApi.ApiSecret = new SecretStruct(model.ZoomApiSecret);
            }

            options.CanvasApi.UseCanvas = model.UseCanvas;
            if (model.CanvasAccessToken != _passwordPlaceholder && !String.IsNullOrEmpty(model.CanvasAccessToken))
            {
                options.CanvasApi.ApiAccessToken = new SecretStruct(model.CanvasAccessToken);
            }

            _secretOptions.Save();
            sizedCache.ResetCache();

            return RedirectToAction("Index", "Home");
        }
    }
}
