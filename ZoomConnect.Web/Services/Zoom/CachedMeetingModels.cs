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
        private const string _cacheKeyMeetings = "foundMeetingModels";

        public CachedMeetingModels(ZoomMeetingFinder meetingFinder, SizedCache cache)
        {
            _meetingFinder = meetingFinder;
            _cache = cache.Cache;
        }

        public List<CourseMeetingDataModel> Meetings
        {
            get
            {
                // see if found meetings are cached
                if (_cache.TryGetValue(_cacheKeyMeetings, out List<CourseMeetingDataModel> cacheEntry))
                {
                    return cacheEntry;
                }

                cacheEntry = _meetingFinder.Meetings;

                Set(cacheEntry);

                return cacheEntry;
            }
        }

        public void Set(List<CourseMeetingDataModel> updatedCacheEntry)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(updatedCacheEntry.Count)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            _cache.Set(_cacheKeyMeetings, updatedCacheEntry, cacheEntryOptions);
        }
    }
}
