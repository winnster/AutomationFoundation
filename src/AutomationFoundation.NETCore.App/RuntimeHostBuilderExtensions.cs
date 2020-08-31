﻿using System;
using AutomationFoundation.Hosting;
using AutomationFoundation.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutomationFoundation
{
    /// <summary>
    /// Contains extension methods for the runtime host builder.
    /// </summary>
    public static class RuntimeHostBuilderExtensions
    {
        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="builder">The builder instance.</param>
        /// <param name="callback">The callback which will configure the application configuration.</param>
        /// <returns>The builder instance.</returns>
        public static IRuntimeHostBuilder ConfigureAppConfiguration(this IRuntimeHostBuilder builder, Action<IConfigurationBuilder> callback)
        {
            return builder.ConfigureAppConfiguration((_, b) => callback(b));
        }

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="builder">The builder instance.</param>
        /// <param name="callback">The callback which will configure the application configuration.</param>
        /// <returns>The builder instance.</returns>
        public static IRuntimeHostBuilder ConfigureAppConfiguration(this IRuntimeHostBuilder builder, Action<IHostingEnvironment, IConfigurationBuilder> callback)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            else if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            return builder.ConfigureServices((sp, services) =>
            {
                var configBuilder = new ConfigurationBuilder();
                callback(sp.GetService<IHostingEnvironment>(), configBuilder);

                var configuration = configBuilder.Build();
                if (configuration == null)
                {
                    throw new BuildException("The configuration was not built as expected.");
                }

                services.AddSingleton<IConfiguration>(_ => configuration);
            });
        }
    }
}