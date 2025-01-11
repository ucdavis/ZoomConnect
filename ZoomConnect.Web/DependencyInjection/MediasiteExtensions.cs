using System;
using Microsoft.Extensions.DependencyInjection;
using MediasiteUtil;
using ZoomConnect.Web.Services.Mediasite;

namespace ZoomConnect.Web.DependencyInjection
{
    public static class MediasiteExtensions
    {
        public static IServiceCollection AddMediasiteServices(this IServiceCollection services)
        {
            services.AddScoped<MediasiteClient>();
            services.AddScoped<MediasiteCourseService>();

            return services;
        }
    }
}
