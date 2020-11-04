using CanvasClient;
using CanvasClient.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using System;
using System.Collections.Generic;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;

namespace ZoomConnect.Web.Services.Canvas
{
    public class CachedCanvasCourses
    {
        private CanvasApi _canvasApi;
        private ZoomOptions _options;
        private IMemoryCache _cache;
        private ILogger<CachedCanvasCourses> _logger;
        private const string _cacheKeyCanvasCourses= "foundCanvasCourses";

        public CachedCanvasCourses(CanvasApi canvasApi, SecretConfigManager<ZoomOptions> optionsManager,
            SizedCache cache, ILogger<CachedCanvasCourses> logger)
        {
            _canvasApi = canvasApi;
            _options = optionsManager.GetValue().Result;
            _cache = cache.Cache;
            _logger = logger;

            _canvasApi.Options = _options.CanvasApi.CreateCanvasOptions();
        }

        public List<Course> Courses
        {
            get
            {
                // see if courses are cached
                if (_cache.TryGetValue(_cacheKeyCanvasCourses, out List<Course> cacheEntry))
                {
                    _logger.LogInformation("Found CanvasCourses in cache");
                    return cacheEntry;
                }

                var canvasAccount = _options.CanvasApi.SelectedAccount;
                var canvasTerm = _options.CanvasApi.EnrollmentTerm;
                cacheEntry = _canvasApi.ListActiveCourses(canvasAccount, canvasTerm);

                Set(cacheEntry);

                return cacheEntry;
            }
        }

        public void Set(List<Course> updatedCacheEntry)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(updatedCacheEntry.Count)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            _cache.Set(_cacheKeyCanvasCourses, updatedCacheEntry, cacheEntryOptions);

            _logger.LogInformation($"Added {updatedCacheEntry.Count} CanvasCourses to cache");
        }
    }
}
