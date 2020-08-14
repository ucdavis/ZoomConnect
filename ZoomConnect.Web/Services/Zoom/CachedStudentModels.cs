using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    public class CachedStudentModels
    {
        private ZoomStudentFinder _studentFinder;
        private IMemoryCache _cache;
        private ILogger<CachedStudentModels> _logger;
        private const string _cacheKeyStudents = "studentModels";

        public CachedStudentModels(ZoomStudentFinder studentFinder, SizedCache cache, ILogger<CachedStudentModels> logger)
        {
            _studentFinder = studentFinder;
            _cache = cache.Cache;
            _logger = logger;
        }

        public List<StudentDataModel> Students
        {
            get
            {
                // see if found students are cached
                if (_cache.TryGetValue(_cacheKeyStudents, out List<StudentDataModel> cacheEntry))
                {
                    _logger.LogInformation("Found StudentDataModels in cache");
                    return cacheEntry;
                }

                cacheEntry = _studentFinder.Students;

                Set(cacheEntry);

                return cacheEntry;
            }
        }

        public void Set(List<StudentDataModel> updatedCacheEntry)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(updatedCacheEntry.Count)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            _cache.Set(_cacheKeyStudents, updatedCacheEntry, cacheEntryOptions);

            _logger.LogInformation($"Added {updatedCacheEntry.Count} StudentDataModels to cache");
        }
    }
}
