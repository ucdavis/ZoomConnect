using System;
using SecretJsonConfig;
using System.Collections.Generic;
using System.Linq;
using ZoomClient.Domain;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    public class ZoomUserCreator
    {
        private SecretConfigManager<ZoomOptions> _optionsManager;
        private ZoomOptions _options;
        private ZoomClient.Zoom _zoomClient;
        private CachedProfModels _cachedProfs;

        public ZoomUserCreator(SecretConfigManager<ZoomOptions> optionsManager, ZoomClient.Zoom zoomClient, CachedProfModels cachedProfs)
        {
            _optionsManager = optionsManager;
            _options = optionsManager.GetValue().Result;
            _zoomClient = zoomClient;
            _cachedProfs = cachedProfs;
        }

        public List<User> CreateLicensedZoomUsers(List<ProfDataModel> profs)
        {
            var profsFromCache = _cachedProfs.Profs;

            var newLicensedUsers = new List<User>(profs.Count);
            foreach (var prof in profs)
            {
                // look for user in Zoom before creating or updating
                var currentUser = _zoomClient.GetUser(prof.primaryEmail.email_address);

                // if confirmed not found, create new user
                User newLicensedUser = null;
                if (currentUser == null)
                {
                    var request = new UserRequest(prof.primaryEmail.email_address,
                        prof.bannerPerson.first_name,
                        prof.bannerPerson.last_name);

                    var result = _zoomClient.CreateUser(request);
                    newLicensedUser = _zoomClient.GetUser(result?.id);
                }
                else if (currentUser.type == PlanType.Basic)
                {
                    // update basic users to licensed
                    var request = new UserUpdate { type = PlanType.Licensed };
                    var updated = _zoomClient.UpdateUserProfile(currentUser.email, request);
                    newLicensedUser = updated ? _zoomClient.GetUser(currentUser.id) : null;
                }

                // stored user result back in cached list
                var foundInCache = profsFromCache.FirstOrDefault(cp =>
                    cp.primaryEmail != null &&
                    cp.primaryEmail.email_address != null &&
                    cp.primaryEmail.email_address == prof.primaryEmail.email_address);
                if (newLicensedUser != null)
                {
                    if (foundInCache != null)
                    {
                        foundInCache.zoomUser = newLicensedUser;
                    }
                    newLicensedUsers.Add(newLicensedUser);
                }
            }

            // save meetings back to cache
            _cachedProfs.Set(profsFromCache);

            return newLicensedUsers;
        }
    }
}
