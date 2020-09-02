using System;
using MediasiteUtil.Models;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Services.Mediasite
{
    public static class MediasiteOptionsExtensions
    {
        public static MediasiteConfig CreateMediasiteConfig(this MediasiteOptions options)
        {
            return new MediasiteConfig
            {
                Endpoint = options.Endpoint,
                Username = options.Username,
                Password = options.Password,
                ApiKey = options.ApiKey,
                RootFolderId = options.RootFolder
            };
        }
    }
}
