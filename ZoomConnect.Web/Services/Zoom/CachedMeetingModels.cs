using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private ILogger<CachedMeetingModels> _logger;
        private const string _cacheKeyCourses = "foundMeetingCourses";
        private const string _cacheKeyMeetings = "foundCourseMeetings";

        public CachedMeetingModels(ZoomMeetingFinder meetingFinder, SizedCache cache, ILogger<CachedMeetingModels> logger)
        {
            _meetingFinder = meetingFinder;
            _cache = cache.Cache;
            _logger = logger;
        }

        public List<CourseMeetingDataModel> Courses
        {
            get
            {
                // see if found courses are cached
                if (_cache.TryGetValue(_cacheKeyCourses, out List<CourseMeetingDataModel> cacheEntry))
                {
                    _logger.LogInformation("Found Meeting Courses in cache");
                    return cacheEntry;
                }

                cacheEntry = _meetingFinder.Courses;
                SetCourses(cacheEntry);
                SetMeetings(_meetingFinder.Meetings);

                return cacheEntry;
            }
        }

        public List<ZoomMeetingCourseModel> Meetings
        {
            get
            {
                // see if found meetings are cached
                if (_cache.TryGetValue(_cacheKeyMeetings, out List<ZoomMeetingCourseModel> cacheEntry))
                {
                    _logger.LogInformation("Found Course Meetings in cache");
                    return cacheEntry;
                }

                cacheEntry = _meetingFinder.Meetings;
                SetMeetings(cacheEntry);
                SetCourses(_meetingFinder.Courses);

                return cacheEntry;
            }
        }

        public void SetCourses(List<CourseMeetingDataModel> updatedCacheEntry)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(updatedCacheEntry.Count)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            _cache.Set(_cacheKeyCourses, updatedCacheEntry, cacheEntryOptions);

            _logger.LogInformation($"Added {updatedCacheEntry.Count} Meeting Courses to cache");
        }

        public void SetMeetings(List<ZoomMeetingCourseModel> updatedCacheEntry)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(updatedCacheEntry.Count)
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            _cache.Set(_cacheKeyMeetings, updatedCacheEntry, cacheEntryOptions);

            _logger.LogInformation($"Added {updatedCacheEntry.Count} Course Meetings to cache");
        }
    }
}
