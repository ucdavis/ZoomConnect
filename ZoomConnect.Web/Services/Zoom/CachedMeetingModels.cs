using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    public class CachedMeetingModels
    {
        private ZoomMeetingFinder _meetingFinder;
        private IMemoryCache _cache;
        private const string _cacheKeyFoundMeetings = "foundMeetingModels";
        private const string _cacheKeyMissingMeetings = "missingMeetingModels";

        public CachedMeetingModels(ZoomMeetingFinder meetingFinder, SizedCache cache)
        {
            _meetingFinder = meetingFinder;
            _cache = cache.Cache;
        }

        public List<CourseMeetingDataModel> FoundMeetings
        {
            get
            {
                // see if found meetings are cached
                if (_cache.TryGetValue(_cacheKeyFoundMeetings, out List<CourseMeetingDataModel> cacheEntry))
                {
                    return cacheEntry;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                cacheEntry = _meetingFinder.FoundMeetings;
                _cache.Set(_cacheKeyFoundMeetings, cacheEntry, cacheEntryOptions);

                _cache.Set(_cacheKeyMissingMeetings, _meetingFinder.MissingMeetings, cacheEntryOptions);

                return cacheEntry;
            }
        }

        public List<CourseMeetingDataModel> MissingMeetings
        {
            get
            {
                // see if missing meetings are cached
                if (_cache.TryGetValue(_cacheKeyMissingMeetings, out List<CourseMeetingDataModel> cacheEntry))
                {
                    return cacheEntry;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                cacheEntry = _meetingFinder.MissingMeetings;
                _cache.Set(_cacheKeyMissingMeetings, cacheEntry, cacheEntryOptions);

                _cache.Set(_cacheKeyFoundMeetings, _meetingFinder.FoundMeetings, cacheEntryOptions);

                return cacheEntry;
            }
        }
    }
}
