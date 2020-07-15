using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace SecretJsonConfig
{
    public static class SecretExtensions
    {
        /// <summary>
        /// Adds SecretConfigManager of specified Type to the DI container.  SecretConfigManager reads and writes to an associated Json file.
        /// The Type's <see cref="SecretStruct"/> members will be encrypted on disk, and decrypted in memory.
        /// </summary>
        /// <typeparam name="TSecret"></typeparam>
        /// <param name="services"></param>
        /// <param name="filename"></param>
        public static void UseSecretJsonConfig<TSecret>(this IServiceCollection services, string filename) where TSecret : new()
        {
            services.AddDataProtection()
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                });

            services.AddScoped<Crypt>();

            services.TryAddScoped<SecretConfigManager<TSecret>>(sp =>
            {
                var fileProvider = new PhysicalFileProvider(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                var crypt = sp.GetRequiredService<Crypt>();
                return new SecretConfigManager<TSecret>(crypt) { SecretFile = fileProvider.GetFileInfo(filename) };
            });
        }
    }
}
