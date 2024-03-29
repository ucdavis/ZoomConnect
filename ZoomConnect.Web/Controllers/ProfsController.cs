﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretJsonConfig;
using ZoomClient;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Models;
using ZoomConnect.Web.Services.Zoom;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    [Authorize("ConfiguredAdmins")]
    [TypeFilter(typeof(CheckRequirements))]
    public class ProfsController : Controller
    {
        private CachedProfModels _userFinder;
        private Zoom _zoomClient;

        public ProfsController(CachedProfModels userFinder, ZoomClient.Zoom zoomClient, SecretConfigManager<ZoomOptions> optionsManager)
        {
            _userFinder = userFinder;
            _zoomClient = zoomClient;

            var _zoomOptions = optionsManager.GetValue().Result;
            _zoomClient.Options = _zoomOptions.ZoomApi.CreateZoomOptions();
        }

        public IActionResult Index()
        {
            var profModels = new SelectedProfsModel
            {
                Profs = _userFinder.Profs
                    .Where(p => p.bannerPerson != null)
                    .Select(p => new ProfViewModel(p))
                    .OrderBy(m => m.Name)
                    .ToList()
            };

            return View(profModels);
        }

        [HttpPost]
        public IActionResult Index(SelectedProfsModel model)
        {
            var planUsage = _zoomClient.GetPlanUsage();
            model.RemainingLicenses = planUsage.plan_base.hosts - planUsage.plan_base.usage;

            model.Profs = RehydrateSelectedProfs(model)
                .Select(p => new ProfViewModel(p) { IsSelected = true })
                .ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(SelectedProfsModel model, [FromServices] ZoomUserCreator userCreator)
        {
            var profs = RehydrateSelectedProfs(model);

            var created = userCreator.CreateLicensedZoomUsers(profs);

            TempData["Message"] = $"{created.Count} Zoom User(s) licensed.";

            return RedirectToAction("Index");
        }

        private List<ProfDataModel> RehydrateSelectedProfs(SelectedProfsModel model)
        {
            var selectedPidms = model.Profs
                .Where(m => m.IsSelected)
                .Select(m => m.Pidm);

            return _userFinder.Profs
                .Where(p => selectedPidms.Contains(p.bannerPerson?.pidm ?? 0))
                .OrderBy(p => p.bannerPerson?.last_name ?? "")
                .ThenBy(p => p.bannerPerson?.first_name ?? "")
                .ToList();
        }
    }
}
