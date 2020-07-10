using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;

namespace ZoomConnect.Web.SecretJsonConfig
{
    public static class SecretExtensions
    {
        public static void UseSecrets<TSecret>(this IServiceCollection services, IFileInfo file) where TSecret : new()
        {
            var secrets = new SecretConfigManager<TSecret> { SecretFile = file };

            services.AddSingleton<SecretConfigManager<TSecret>>(secrets);
        }
    }
}
