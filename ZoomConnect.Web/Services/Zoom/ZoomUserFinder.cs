﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<goremal> _alternateEmails { get; set; }

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
            _alternateEmails = new List<goremal>();

            // find preferred emails first
            var allEmails = _profEmailRepo.GetAll()
                .OrderByDescending(e => e.preferred_ind)
                .ToList();

            allEmails.ForEach(e =>
            {
                // skip any emails for profs who are already found in Zoom
                if (_alternateEmails.Contains(e)) { return; }

                // check Zoom for prof email
                var zoomUser = _zoomClient.GetUser(e.email_address);
                if (zoomUser != null)
                {
                    // create a new prof model with email connected to zoom
                    var prof = new ProfDataModel
                    {
                        primaryEmail = e,
                        zoomUser = zoomUser
                    };

                    // record alternate emails so we can skip them later
                    var altEmails = allEmails.Where(alt => alt.pidm == e.pidm && alt.email_address != e.email_address);
                    prof.otherEmails.AddRange(altEmails);
                    _alternateEmails.AddRange(altEmails);

                    _foundProfs.Add(prof);
                }
                else
                {
                    _missingProfs.Add(e);
                }
            });
        }
    }
}
