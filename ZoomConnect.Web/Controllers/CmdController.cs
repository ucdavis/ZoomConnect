using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZoomConnect.Web.Filters;

namespace ZoomConnect.Web.Controllers
{
    [CorsAllowAll]
    [TypeFilter(typeof(CmdKeyAuthorize))]
    public class CmdController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CloudDownload()
        {
            return Content("CloudDownloader");
        }
    }
}
