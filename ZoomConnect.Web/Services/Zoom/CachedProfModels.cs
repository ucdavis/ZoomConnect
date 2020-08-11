using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private ILogger<CachedProfModels> _logger;
        private const string _cacheKeyProfs = "profModels";

        public CachedProfModels(ZoomUserFinder userFinder, SizedCache cache, ILogger<CachedProfModels> logger)
        {
            _userFinder = userFinder;
            _cache = cache.Cache;
            _logger = logger;
        }

        public List<ProfDataModel> Profs
        {
            get
            {
                // see if found profs are cached
                if (_cache.TryGetValue(_cacheKeyProfs, out List<ProfDataModel> cacheEntry))
                {
                    _logger.LogInformation("Found ProfDataModels in cache");
                    return cacheEntry;
                }

                cacheEntry = _userFinder.Profs;

                Set(cacheEntry);

                return cacheEntry;
            }
        }

        public void Set(List<ProfDataModel> updatedCacheEntry)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(updatedCacheEntry.Count)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            _cache.Set(_cacheKeyProfs, updatedCacheEntry, cacheEntryOptions);

            _logger.LogInformation($"Added {updatedCacheEntry.Count} ProfDataModels to cache");
        }
    }
}
