using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Contentful.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Contentful.Core;

namespace Contentful.AspNetCore
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddContentful(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddOptions();
            services.Configure<ContentfulOptions>(configuration.GetSection("ContentfulOptions"));
            services.TryAddSingleton<HttpClient>();
            services.TryAddTransient<IContentfulClient, ContentfulClient>();
            services.TryAddTransient<IContentfulManagementClient, ContentfulManagementClient>();
            return services;
        }
    }
}
