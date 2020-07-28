using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Services.Zoom;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(CheckRequirements))]
    public class ProfsController : Controller
    {
        private CachedProfModels _userFinder;

        public ProfsController(CachedProfModels userFinder)
        {
            _userFinder = userFinder;
        }

        public IActionResult Index()
        {
            var profModels = _userFinder.Profs.Select(p => new ProfViewModel(p))
                .OrderBy(m => m.Name)
                .ToList();

            return View(profModels);
        }

        [HttpPost]
        public IActionResult Index(List<ProfViewModel> models)
        {
            ViewData["SelectedCount"] = models.Where(m => m.IsSelected).Count();

            var selectedPidms = models.Where(m => m.IsSelected)
                .Select(m => m.Pidm);

            var profModels = _userFinder.Profs.Select(p => new ProfViewModel(p))
                .Where(p => selectedPidms.Contains(p.Pidm))
                .ToList();

            return View(profModels);
        }
    }
}
