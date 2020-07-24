using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    /// <summary>
    /// Finds Zoom User associated with each email address of an assigned prof.
    /// </summary>
    public class ZoomUserFinder
    {
        private CachedRepository<goremal> _profEmailRepo;
        private ZoomClient.Zoom _zoomClient;

        public ZoomUserFinder(CachedRepository<goremal> profEmailRepo, ZoomClient.Zoom zoomClient)
        {
            _profEmailRepo = profEmailRepo;
            _zoomClient = zoomClient;
        }

        /// <summary>
        /// Searches for Zoom User by each email in goremal
        /// </summary>
        /// <returns></returns>
        public List<ProfDataModel> Find()
        {
            var models = new List<ProfDataModel>();

            _profEmailRepo
                .GetAll()
                .ForEach(e =>
                {
                    var zoomUser = _zoomClient.GetUser(e.email_address);
                    if (zoomUser != null)
                    {
                        models.Add(new ProfDataModel
                        {
                            primaryEmail = e,
                            zoomUser = zoomUser
                        });
                    }
                });

            return models;
        }
    }
}
