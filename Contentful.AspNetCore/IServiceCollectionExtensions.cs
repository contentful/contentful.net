using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Contentful.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Contentful.Core;
using Microsoft.Extensions.Options;

namespace Contentful.AspNetCore
{
    /// <summary>
    /// Extension methods for IServiceCollection.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Contentful services to the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="configuration">The IConfigurationRoot used to retrieve configuration from.</param>
        /// <returns>The IServiceCollection.</returns>
        public static IServiceCollection AddContentful(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddOptions();
            services.Configure<ContentfulOptions>(configuration.GetSection("ContentfulOptions"));
            services.TryAddSingleton<HttpClient>();
            services.TryAddTransient<IContentfulClient>((sp) => {
                var options = sp.GetService<IOptions<ContentfulOptions>>().Value;
                var client = sp.GetService<HttpClient>();
                return new ContentfulClient(client, options);
            });
            services.TryAddTransient<IContentfulManagementClient>((sp) => {
                var options = sp.GetService<IOptions<ContentfulOptions>>().Value;
                var client = sp.GetService<HttpClient>();
                return new ContentfulManagementClient(client, options);
            });
            return services;
        }

        /// <summary>
        /// Adds Contentful services to the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="configuration">The IConfiguration used to retrieve configuration from.</param>
        /// <returns>The IServiceCollection.</returns>
        public static IServiceCollection AddContentful(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ContentfulOptions>(configuration.GetSection("ContentfulOptions"));
            services.TryAddSingleton<HttpClient>();
            services.TryAddTransient<IContentfulClient>((sp) => {
                var options = sp.GetService<IOptions<ContentfulOptions>>().Value;
                var client = sp.GetService<HttpClient>();
                return new ContentfulClient(client, options);
            });
            services.TryAddTransient<IContentfulManagementClient>((sp) => {
                var options = sp.GetService<IOptions<ContentfulOptions>>().Value;
                var client = sp.GetService<HttpClient>();
                return new ContentfulManagementClient(client, options);
            });
            return services;
        }
    }
}
