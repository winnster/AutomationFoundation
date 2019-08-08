﻿using System;
using AutomationFoundation.Hosting.Abstractions;
using AutomationFoundation.Hosting.Abstractions.Builder;
using ConsoleRunner.Abstractions;
using ConsoleRunner.Abstractions.DataAccess;
using ConsoleRunner.Infrastructure.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleRunner
{
    internal partial class Startup : IStartup
    {
        public void ConfigureProcessors(IRuntimeBuilder runtimeBuilder)
        {
            if (runtimeBuilder == null)
            {
                throw new ArgumentNullException(nameof(runtimeBuilder));
            }

            using var scope = runtimeBuilder.ApplicationServices.CreateScope();
            ConfigureProcessors(runtimeBuilder, scope.ServiceProvider);
        }

        private static void ConfigureProcessors(IRuntimeBuilder runtimeBuilder, IServiceProvider serviceProvider)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            
            var configs = unitOfWork.AppProcessors.GetProcessorsForMachine(Environment.MachineName);

            foreach (var config in configs)
            {
                var builder = GetBuilder(config.ProcessorType);
                if (builder == null)
                {
                    throw new InvalidOperationException("The processor builder was not created.");
                }

                var processor = builder.Build(runtimeBuilder, config);
                if (processor == null)
                {
                    throw new InvalidOperationException("The processor was not built.");
                }

                runtimeBuilder.RegisterProcessor(processor);
            }
        }

        private static IProcessorBuilder GetBuilder(ProcessorTypeEnum processorType)
        {
            switch (processorType)
            {
                case ProcessorTypeEnum.ProducerConsumer:
                    return new ProducerConsumerProcessorBuilder();

                case ProcessorTypeEnum.ScheduledJob:
                case ProcessorTypeEnum.Task:
                    throw new NotSupportedException($"The processor type '{processorType}' is not supported.");
            }

            return null;
        }
    }
}