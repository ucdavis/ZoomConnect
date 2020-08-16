using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZoomClient;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Filters;

namespace ZoomConnect.Web.Controllers
{
    [CorsAllowAll]
    [TypeFilter(typeof(CmdKeyAuthorize))]
    public class CmdController : Controller
    {
        private Zoom _zoomClient;
        private CachedRepository<goremal> _goremal;

        public CmdController(ZoomClient.Zoom zoomClient, CachedRepository<goremal> goremal)
        {
            _zoomClient = zoomClient;
            _goremal = goremal;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CloudDownload()
        {
            // get all prof emails
            var profEmails = _goremal.GetAll()
                .Select(e => e.email_address.ToLower())
                .ToList();

            // get all recordings for account
            var recordings = _zoomClient.GetCloudRecordingsForAccount();

            // filter recordings for found profs
            var profRecordings = recordings.Where(r => profEmails.Contains(r.host_email.ToLower()))
                .ToList();

            return Content($"CloudDownloader found prof recordings for: {String.Join(",", profRecordings.Select(r => r.host_email))}");
        }
    }
}
