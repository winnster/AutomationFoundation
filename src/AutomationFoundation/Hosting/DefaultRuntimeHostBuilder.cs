using System;
using System.Collections.Generic;
using AutomationFoundation.Hosting.Abstractions;
using AutomationFoundation.Hosting.Abstractions.Builders;
using AutomationFoundation.Runtime.Abstractions.Builders;
using AutomationFoundation.Runtime.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutomationFoundation.Hosting
{
    /// <summary>
    /// Provides the default runtime host builder.
    /// </summary>
    public class DefaultRuntimeHostBuilder : IRuntimeHostBuilder
    {
        private readonly IList<Action<IServiceCollection>> callbacks = new List<Action<IServiceCollection>>();

        private Action<IHostingEnvironmentBuilder> hostingEnvironmentConfigurationCallback;
        private bool startupHasBeenConfigured;

        /// <inheritdoc />
        public IRuntimeHostBuilder ConfigureHostingEnvironment(Action<IHostingEnvironmentBuilder> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            hostingEnvironmentConfigurationCallback = callback;
            return this;
        }

        /// <inheritdoc />
        public IRuntimeHostBuilder ConfigureServices(Action<IServiceCollection> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            callbacks.Add(callback);
            return this;
        }

        /// <inheritdoc />
        public IRuntimeHostBuilder UseStartup<TStartup>() where TStartup : IStartup
        {
            callbacks.Add(services => services.AddScoped(typeof(IStartup), typeof(TStartup)));
            startupHasBeenConfigured = true;

            return this;
        }

        /// <inheritdoc />
        public IRuntimeHostBuilder UseStartup(IStartup startup)
        {
            if (startup == null)
            {
                throw new ArgumentNullException(nameof(startup));
            }

            callbacks.Add(services => services.AddSingleton(typeof(IStartup), (sp) => startup));
            startupHasBeenConfigured = true;

            return this;
        }

        /// <inheritdoc />
        public IRuntimeHost Build()
        {
            GuardStartupMustBeConfigured();

            var services = CreateServiceCollection();

            ConfigureHostingEnvironmentImpl(services);
            ConfigureServiceCollection(services);

            IServiceProvider sp = null;

            try
            {
                sp = BuildServiceProvider(services);

                var startup = ResolveStartupInstance(sp);
                if (startup == null)
                {
                    throw new BuildException("The startup instance was not resolved.");
                }

                var applicationServices = ConfigureApplicationServices(startup, services);
                if (applicationServices == null)
                {
                    throw new BuildException("The services were not configured.");
                }

                var hostingEnvironment = applicationServices.GetRequiredService<IHostingEnvironment>();

                var runtimeBuilder = CreateRuntimeBuilder(applicationServices);
                startup.ConfigureProcessors(runtimeBuilder, hostingEnvironment);

                var runtime = runtimeBuilder.Build();
                if (runtime == null)
                {
                    throw new BuildException("The runtime could not be built.");
                }

                return new RuntimeHost(
                    runtime,
                    hostingEnvironment,
                    applicationServices,
                    applicationServices.GetService<ILogger<RuntimeHost>>());
            }
            finally
            {
                (sp as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        /// Creates the service collection.
        /// </summary>
        /// <returns>The new service collection instance.</returns>
        protected virtual IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        /// <summary>
        /// Builds a service provider from a collection of services.
        /// </summary>
        /// <param name="services">The service collection from which to build the provider.</param>
        /// <returns>The service provider.</returns>
        protected virtual IServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Configures the environment.
        /// </summary>
        /// <param name="serviceCollection">The service collection to configure.</param>
        protected virtual void ConfigureHostingEnvironmentImpl(IServiceCollection serviceCollection)
        {
            var builder = new DefaultHostingEnvironmentBuilder();
            hostingEnvironmentConfigurationCallback?.Invoke(builder);

            var environment = builder.Build();
            if (environment == null)
            {
                throw new BuildException("The environment was not built.");
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IHostingEnvironment), environment));
        }

        /// <summary>
        /// Configures the service collection used for dependency injection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to configure.</param>
        protected virtual void ConfigureServiceCollection(IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            foreach (var callback in callbacks)
            {
                callback(serviceCollection);
            }
        }

        /// <summary>
        /// Creates a runtime builder.
        /// </summary>
        /// <param name="applicationServices">The application services to use with the builder.</param>
        /// <returns>The runtime builder instance.</returns>
        protected virtual IRuntimeBuilder CreateRuntimeBuilder(IServiceProvider applicationServices)
        {
            if (applicationServices == null)
            {
                throw new ArgumentNullException(nameof(applicationServices));
            }

            return new DefaultRuntimeBuilder(applicationServices);
        }

        /// <summary>
        /// Resolves the startup instance.
        /// </summary>
        /// <param name="serviceProvider">The service provider to use to resolve the startup instance.</param>
        /// <returns>An <see cref="IStartup"/> to handle startup operations.</returns>
        protected virtual IStartup ResolveStartupInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return serviceProvider.GetRequiredService<IStartup>();
        }

        /// <summary>
        /// Configures the application services.
        /// </summary>
        /// <param name="startup">The startup class handling startup operations.</param>
        /// <param name="serviceCollection">The collection of services for the application.</param>
        /// <returns>The service provider containing the application services.</returns>
        protected virtual IServiceProvider ConfigureApplicationServices(IStartup startup, IServiceCollection serviceCollection)
        {
            if (startup == null)
            {
                throw new ArgumentNullException(nameof(startup));
            }
            else if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return startup.TryConfigureContainer(serviceCollection);
        }

        /// <summary>
        /// Ensures the startup has been configured.
        /// </summary>
        /// <exception cref="BuildException">The startup has not been configured.</exception>
        protected void GuardStartupMustBeConfigured()
        {
            if (!startupHasBeenConfigured)
            {
                throw new BuildException("The startup must be configured.");
            }
        }
    }
}