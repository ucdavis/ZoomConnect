using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Core.Config;
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
        private CachedRepository<spriden> _personRepo;
        private CachedRepository<goremal> _profEmailRepo;
        private CachedRepository<sirasgn> _assignmentRepo;
        private CachedRepository<ssbsect> _courseRepo;
        private ZoomClient.Zoom _zoomClient;
        private ZoomOptions _zoomOptions;

        private List<ProfDataModel> _profs { get; set; }

        public ZoomUserFinder(ZoomClient.Zoom zoomClient, SecretConfigManager<ZoomOptions> optionsManager,
            CachedRepository<spriden> personRepo, CachedRepository<goremal> profEmailRepo,
            CachedRepository<sirasgn> assignmentRepo, CachedRepository<ssbsect> courseRepo)
        {
            _zoomClient = zoomClient;
            _zoomOptions = optionsManager.GetValue().Result;
            _personRepo = personRepo;
            _profEmailRepo = profEmailRepo;
            _assignmentRepo = assignmentRepo;
            _courseRepo = courseRepo;

            _zoomClient.Options = _zoomOptions.ZoomApi.CreateZoomOptions();
        }

        /// <summary>
        /// Profs linked to Zoom users by email
        /// </summary>
        public List<ProfDataModel> Profs
        {
            get
            {
                if (_profs == null)
                {
                    Find();
                }

                return _profs;
            }
        }

        /// <summary>
        /// Searches for Zoom User by each email in goremal.
        /// Looks for Preferred email first.
        /// When found, all alternate emails are collated with found prof and not checked in Zoom.
        /// </summary>
        /// <returns></returns>
        private void Find()
        {
            _profs = new List<ProfDataModel>();
            var _allFound = new List<string>();

            // find all person rows
            var bannerPeople = _personRepo.GetAll();

            // find preferred emails first
            var allEmails = _profEmailRepo.GetAll()
                .OrderByDescending(e => e.preferred_ind)
                .ToList();

            // add extra prof emails with unique pidms so they are not grouped with anyone else
            var extraProfs = (_zoomOptions.ExtraProfEmails ?? "")
                .ToLower()
                .Split(new[] { ";", ",", " " }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            for (var i = 0; i < extraProfs.Length; i++)
            {
                allEmails.Add(new goremal
                {
                     pidm = -1 - i,
                     email_address = extraProfs[i],
                     status_ind = "A",
                     preferred_ind = "Y"
                });
            }

            allEmails.ForEach(e =>
            {
                // skip any emails for profs who are already found in Zoom
                if (_allFound.Contains(e.email_address)) { return; }

                // check Zoom for prof email
                var zoomUser = _zoomClient.GetUser(e.email_address);
                if (zoomUser == null) { return; }

                // create a new prof model with email connected to zoom
                var prof = new ProfDataModel
                {
                    bannerPerson = bannerPeople.FirstOrDefault(p => p.pidm == e.pidm),
                    primaryEmail = e,
                    zoomUser = zoomUser
                };
                prof.AddAssignments(_assignmentRepo, _courseRepo);

                // get prof's alternate emails so we can skip them later
                var altEmails = allEmails.Where(alt => alt.pidm == e.pidm && alt.email_address != e.email_address);
                prof.otherEmails.AddRange(altEmails);
                _allFound.Add(e.email_address);
                _allFound.AddRange(altEmails.Select(a => a.email_address));

                _profs.Add(prof);
            });

            // collate missing emails into per-prof model
            var missingProfs = allEmails
                .Where(e => !_allFound.Contains(e.email_address))
                .GroupBy(e => e.pidm, e => e, (key, grp) => new ProfDataModel
                {
                    bannerPerson = bannerPeople.FirstOrDefault(p => p.pidm == key),
                    primaryEmail = grp.OrderByDescending(g => g.preferred_ind).First(),
                    otherEmails = grp.OrderByDescending(g => g.preferred_ind).Skip(1).ToList()
                })
                .ToList();

            // include profs without any email (The Staff)
            missingProfs.AddRange(_personRepo.GetAll()
                .Where(p => !allEmails.Select(e => e.pidm).Contains(p.pidm))
                .Select(p => new ProfDataModel { bannerPerson = p }));

            // include assignments for each prof
            missingProfs.ForEach(m => m.AddAssignments(_assignmentRepo, _courseRepo));

            // add to full list of profs
            _profs.AddRange(missingProfs);
        }
    }
}
