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
        private List<ProfDataModel> _foundProfs { get; set; }
        private List<goremal> _missingProfs { get; set; }

        public ZoomUserFinder(CachedRepository<goremal> profEmailRepo, ZoomClient.Zoom zoomClient)
        {
            _profEmailRepo = profEmailRepo;
            _zoomClient = zoomClient;

            Find();
        }

        /// <summary>
        /// Profs linked to Zoom users by email
        /// </summary>
        public List<ProfDataModel> FoundProfs
        {
            get
            {
                return _foundProfs;
            }
        }

        /// <summary>
        /// Profs teaching this term not matched by email with a User in your Zoom account.
        /// </summary>
        public List<goremal> MissingProfs
        {
            get
            {
                return _missingProfs;
            }
        }

        /// <summary>
        /// Searches for Zoom User by each email in goremal
        /// </summary>
        /// <returns></returns>
        private void Find()
        {
            _foundProfs = new List<ProfDataModel>();
            _missingProfs = new List<goremal>();

            _profEmailRepo
                .GetAll()
                .ForEach(e =>
                {
                    var zoomUser = _zoomClient.GetUser(e.email_address);
                    if (zoomUser != null)
                    {
                        _foundProfs.Add(new ProfDataModel
                        {
                            primaryEmail = e,
                            zoomUser = zoomUser
                        });
                    }
                    else
                    {
                        _missingProfs.Add(e);
                    }
                });
        }
    }
}
