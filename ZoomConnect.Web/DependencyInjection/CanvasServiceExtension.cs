using System;
using CanvasClient;
using ZoomConnect.Web.Services.Canvas;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CanvasServiceExtension
    {
        public static IServiceCollection AddCanvasZoomServices(this IServiceCollection services)
        {
            services.AddScoped<CanvasApi>();
            services.AddScoped<CachedCanvasCourses>();
            services.AddScoped<CalendarEventFinder>();
            services.AddScoped<CanvasEventCreator>();
            services.AddScoped<CanvasScheduledEventUpdater>();

            return services;
        }
    }
}
