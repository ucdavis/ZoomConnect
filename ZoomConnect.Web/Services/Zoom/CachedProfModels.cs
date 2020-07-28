using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    public class CachedProfModels
    {
        private ZoomUserFinder _userFinder;
        private IMemoryCache _cache;
        private const string _cacheKeyProfs = "profModels";

        public CachedProfModels(ZoomUserFinder userFinder, SizedCache cache)
        {
            _userFinder = userFinder;
            _cache = cache.Cache;
        }

        public List<ProfDataModel> Profs
        {
            get
            {
                // see if found profs are cached
                if (_cache.TryGetValue(_cacheKeyProfs, out List<ProfDataModel> cacheEntry))
                {
                    return cacheEntry;
                }

                cacheEntry = _userFinder.Profs;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(cacheEntry.Count)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(_cacheKeyProfs, cacheEntry, cacheEntryOptions);

                return cacheEntry;
            }
        }
    }
}
