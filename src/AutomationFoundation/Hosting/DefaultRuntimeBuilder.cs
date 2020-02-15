using System;
using System.Collections.Generic;
using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions;
using AutomationFoundation.Runtime.Abstractions.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutomationFoundation.Hosting
{
    internal class DefaultRuntimeBuilder : IRuntimeBuilder
    {
        private readonly IList<IProcessorBuilder> processorBuilders = new List<IProcessorBuilder>();
        private readonly IList<IProcessor> processors = new List<IProcessor>();

        public IServiceProvider ApplicationServices { get; }

        public DefaultRuntimeBuilder(IServiceProvider applicationServices)
        {
            ApplicationServices = applicationServices ?? throw new ArgumentNullException(nameof(applicationServices));
        }

        public IRuntimeBuilder RegisterProcessor(IProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            if (processors.Contains(processor))
            {
                throw new ArgumentException("The processor has already been added.", nameof(processor));
            }

            processors.Add(processor);
            return this;
        }

        public IRuntimeBuilder RegisterProcessor(IProcessorBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (processorBuilders.Contains(builder))
            {
                throw new ArgumentException("The builder has already been added.", nameof(builder));
            }

            processorBuilders.Add(builder);
            return this;
        }

        public IRuntime Build()
        {
            AutomationRuntime runtime = null;

            try
            {
                runtime = new AutomationRuntime(ApplicationServices.GetRequiredService<ILogger<AutomationRuntime>>());

                foreach (var processorBuilder in processorBuilders)
                {
                    var processor = processorBuilder.Build();
                    if (processor == null)
                    {
                        throw new InvalidOperationException("The processor was not built.");
                    }

                    runtime.Add(processor);
                }

                foreach (var processor in processors)
                {
                    runtime.Add(processor);
                }

                return runtime;
            }
            catch (Exception)
            {
                runtime?.Dispose();
                throw;
            }
        }
    }
}