using System;
using ZoomConnect.Web.Filters;
using ZoomConnect.Web.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CmdKeyExtension
    {
        public static IServiceCollection AddCommandKey(this IServiceCollection services)
        {
            services.AddTransient<CmdKeyService>();
            services.AddTransient<CmdKeyAuthorize>();
            return services;
        }
    }
}
