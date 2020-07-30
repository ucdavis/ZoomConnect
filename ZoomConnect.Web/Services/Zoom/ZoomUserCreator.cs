using Microsoft.Extensions.Caching.Memory;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public List<UserInfo> CreateZoomUsers(List<ProfDataModel> profs)
        {
            var profsFromCache = _cachedProfs.Profs;

            var createdUsers = new List<UserInfo>(profs.Count);
            foreach (var prof in profs)
            {
                var request = new UserRequest(prof.primaryEmail.email_address,
                    prof.bannerPerson.first_name,
                    prof.bannerPerson.last_name);

                var result = _zoomClient.CreateUser(request);

                // stored created user result back in cached list
                var createdUser = _zoomClient.GetUser(result?.id);
                var foundInCache = profsFromCache.FirstOrDefault(cp => cp.primaryEmail.email_address == createdUser?.email);
                if (foundInCache != null)
                {
                    foundInCache.zoomUser = createdUser;
                }
                createdUsers.Add(result);
            }

            // save meetings back to cache
            _cachedProfs.Set(profsFromCache);

            return createdUsers;
        }
    }
}
