using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Services.Zoom;

namespace ZoomConnect.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(CheckRequirements))]
    public class ProfsController : Controller
    {
        private CachedRepository<goremal> _emailTable;
        private CachedProfModels _userFinder;

        public ProfsController(CachedRepository<goremal> emailTable, CachedProfModels userFinder)
        {
            _emailTable = emailTable;
            _userFinder = userFinder;
        }

        public IActionResult Index()
        {
            var goremalRows = _emailTable.GetAll();
            ViewData["GoremalCount"] = goremalRows.Count;

            return View(_userFinder);
        }
    }
}
