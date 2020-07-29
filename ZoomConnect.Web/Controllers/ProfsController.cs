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
            var profModels = new SelectedProfsModel
            {
                Profs = _userFinder.Profs.Select(p => new ProfViewModel(p))
                    .OrderBy(m => m.Name)
                    .ToList()
            };

            return View(profModels);
        }

        [HttpPost]
        public IActionResult Index(SelectedProfsModel model, [FromServices] ZoomClient.Zoom zoomClient)
        {
            var planUsage = zoomClient.GetPlanUsage();
            model.RemainingLicenses = planUsage.plan_base.hosts - planUsage.plan_base.usage;

            var selectedPidms = model.Profs.Where(m => m.IsSelected)
                .Select(m => m.Pidm);

            model.Profs = _userFinder.Profs.Select(p => new ProfViewModel(p) { IsSelected = true })
                .Where(p => selectedPidms.Contains(p.Pidm))
                .ToList();

            return View(model);
        }
    }
}
