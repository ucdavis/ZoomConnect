using System;
using Microsoft.Extensions.DependencyInjection;
using MediasiteUtil;

namespace ZoomConnect.Web.DependencyInjection
{
    public static class MediasiteExtensions
    {
        public static IServiceCollection AddMediasiteServices(this IServiceCollection services)
        {
            services.AddScoped<MediasiteClient>();

            return services;
        }
    }
}
