﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                ZoomApiSecret = options.ZoomApi?.ApiSecret
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(BannerOptionsViewModel model, [FromServices] SizedCache sizedCache)
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

            _secretOptions.Save();
            sizedCache.ResetCache();

            return RedirectToAction("Index", "Home");
        }
    }
}