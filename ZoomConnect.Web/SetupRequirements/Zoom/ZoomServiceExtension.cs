using System;
using ZoomConnect.Web.Services.Zoom;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ZoomServiceExtension
    {
        public static IServiceCollection AddZoomServices(this IServiceCollection services)
        {
            services.AddScoped<ZoomClient.Zoom>();
            services.AddScoped<ZoomUserFinder>();

            return services;
        }
    }
}
