using System;
using AutomationFoundation.Features.ProducerConsumer;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using AutomationFoundation.Features.ProducerConsumer.Engines;
using AutomationFoundation.Features.ProducerConsumer.Engines.Configuration;
using AutomationFoundation.Features.ProducerConsumer.Resolvers;
using AutomationFoundation.Features.ProducerConsumer.Strategies;
using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions.Builders;
using AutomationFoundation.Runtime.Abstractions.Synchronization;
using AutomationFoundation.Runtime.Threading;
using AutomationFoundation.Runtime.Threading.Primitives;
using ConsoleRunner.Abstractions;
using ConsoleRunner.Infrastructure.Diagnostics;
using ConsoleRunner.Infrastructure.IO;
using ConsoleRunner.Infrastructure.WorkProcessors;
using ConsoleRunner.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleRunner.Infrastructure.Builders
{
    public class ProducerConsumerProcessorBuilder : IApplicationProcessorBuilder
    {
        public Processor Build(IRuntimeBuilder runtimeBuilder, AppProcessor config)
        {
            if (runtimeBuilder == null)
            {
                throw new ArgumentNullException(nameof(runtimeBuilder));
            }
            else if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var producerEngine1 = BuildProducerEngine(runtimeBuilder, null);
            var producerEngine2 = BuildProducerEngine(runtimeBuilder, null);

            var consumerEngine = BuildConsumerEngine();

            return new ProducerConsumerProcessor<int>(
                config.Name,
                new CancellationSourceFactory(),
                new[] { producerEngine1, producerEngine2 },
                consumerEngine,
                runtimeBuilder.ApplicationServices.GetRequiredService<ILogger<ProducerConsumerProcessor<int>>>());
        }

        private IConsumerEngine<int> BuildConsumerEngine()
        {
            return new SynchronousConsumerEngine<int>(
                WorkerPool.Create(),
                new DefaultConsumerExecutionStrategy<int>(
                    new CallbackConsumerResolver<int>(
                        (context) => new IntConsumer())));
        }

        private IProducerEngine<int> BuildProducerEngine(IRuntimeBuilder runtimeBuilder, ISynchronizationPolicy synchronizationPolicy)
        {
            return new ScheduledProducerEngine<int>(
                new DefaultProducerExecutionStrategy<int>(
                    runtimeBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>(),
                    new CallbackProducerResolver<int>(
                        (context) => new RandomIntProducer()),
                    synchronizationPolicy,
                    false),
                new CancellationSourceFactory(),
                new StrategyErrorHandler(
                    new LogToConsoleErrorStrategy(new ConsoleWriter(), LoggingLevel.All)),
                new PollingScheduler(TimeSpan.FromSeconds(5)),
                new ScheduledEngineOptions
                {
                    ContinueUntilEmpty = true
                });
        }
    }
}