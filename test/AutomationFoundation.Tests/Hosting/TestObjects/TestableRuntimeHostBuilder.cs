﻿using System;
using AutomationFoundation.Runtime.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace AutomationFoundation.Hosting.TestObjects
{
    internal class TestableRuntimeHostBuilder : DefaultRuntimeHostBuilder
    {
        private readonly Func<IServiceProvider, IStartup> startupResolver;
        private readonly Func<IStartup, IServiceCollection, IServiceProvider> applicationServicesResolver;
        private readonly Func<IServiceProvider, IRuntimeBuilder> runtimeBuilderResolver;
        private readonly Func<IServiceCollection> serviceCollection;

        public TestableRuntimeHostBuilder(Func<IServiceProvider, IStartup> startupResolver = null, Func<IStartup, IServiceCollection, IServiceProvider> applicationServicesResolver = null, Func<IServiceProvider, IRuntimeBuilder> runtimeBuilderResolver = null, Func<IServiceCollection> serviceCollection = null)
        {
            this.startupResolver = startupResolver;
            this.applicationServicesResolver = applicationServicesResolver;
            this.runtimeBuilderResolver = runtimeBuilderResolver;
            this.serviceCollection = serviceCollection;
        }

        protected override IStartup ResolveStartupInstance(IServiceProvider serviceProvider)
        {
            if (startupResolver != null)
            {
                return startupResolver(serviceProvider);
            }

            return base.ResolveStartupInstance(serviceProvider);
        }

        protected override IServiceProvider ConfigureApplicationServices(IStartup startup, IServiceCollection serviceCollection)
        {
            if (applicationServicesResolver != null)
            {
                return applicationServicesResolver(startup, serviceCollection);
            }

            return base.ConfigureApplicationServices(startup, serviceCollection);
        }

        protected override IRuntimeBuilder CreateRuntimeBuilder(IServiceProvider applicationServices)
        {
            if (runtimeBuilderResolver != null)
            {
                return runtimeBuilderResolver(applicationServices);
            }

            return base.CreateRuntimeBuilder(applicationServices);
        }

        protected override IServiceCollection CreateServiceCollection()
        {
            if (serviceCollection != null)
            {
                return serviceCollection();
            }

            return base.CreateServiceCollection();
        }
    }
}