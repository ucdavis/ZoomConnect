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
        private const string _cacheKeyFoundProfs = "foundProfModels";
        private const string _cacheKeyMissingProfs = "missingProfModels";

        public CachedProfModels(ZoomUserFinder userFinder, SizedCache cache)
        {
            _userFinder = userFinder;
            _cache = cache.Cache;
        }

        public List<ProfDataModel> FoundProfs
        {
            get
            {
                // see if found profs are cached
                if (_cache.TryGetValue(_cacheKeyFoundProfs, out List<ProfDataModel> cacheEntry))
                {
                    return cacheEntry;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                cacheEntry = _userFinder.FoundProfs;
                _cache.Set(_cacheKeyFoundProfs, cacheEntry, cacheEntryOptions);

                _cache.Set(_cacheKeyMissingProfs, _userFinder.MissingProfs, cacheEntryOptions);

                return cacheEntry;
            }
        }

        public List<ProfDataModel> MissingProfs
        {
            get
            {
                // see if missing profs are cached
                if (_cache.TryGetValue(_cacheKeyMissingProfs, out List<ProfDataModel> cacheEntry))
                {
                    return cacheEntry;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                cacheEntry = _userFinder.MissingProfs;
                _cache.Set(_cacheKeyMissingProfs, cacheEntry, cacheEntryOptions);

                _cache.Set(_cacheKeyFoundProfs, _userFinder.FoundProfs, cacheEntryOptions);

                return cacheEntry;
            }
        }
    }
}
