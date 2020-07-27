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
    public class CoursesController : Controller
    {
        private CachedMeetingModels _meetingModels;
        private CachedRepository<ssrmeet> _meetingTable;

        public CoursesController(CachedMeetingModels meetingModels, CachedRepository<ssrmeet> meetingTable)
        {
            _meetingModels = meetingModels;
            _meetingTable = meetingTable;
        }

        public IActionResult Index()
        {
            ViewData["RowCount"] = _meetingTable.GetAll().Count;

            return View(_meetingModels);
        }
    }
}
