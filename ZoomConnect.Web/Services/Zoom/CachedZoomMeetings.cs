using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoomClient;
using ZoomClient.Domain;
using ZoomConnect.Web.Banner.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace ZoomConnect.Web.Services.Zoom
{
    public class CachedZoom
    {
        private ZoomClient.Zoom _zoomClient;
        private IMemoryCache _cache;
        private const string _cacheKeyMeetings = "zoomMeetings";

        public CachedZoom(ZoomClient.Zoom zoomClient, SizedCache cache)
        {
            _zoomClient = zoomClient;
            _cache = cache.Cache;
        }

        public List<Meeting> Meetings
        {
            get
            {
                // see if meetings are cached
                if (_cache.TryGetValue(_cacheKeyMeetings, out List<Meeting> cacheEntry))
                {
                    return cacheEntry;
                }

                // TODO pull from Zoom Services once complete
                // otherwise get meetings from Zoom into cache
                //cacheEntry = _repository.TestConnection();

                //var cacheEntryOptions = new MemoryCacheEntryOptions()
                //    .SetSize(1)
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                //_cache.Set(_cacheKeyTest, cacheEntry, cacheEntryOptions);

                //return cacheEntry;
                return null;
            }
        }
    }
}
