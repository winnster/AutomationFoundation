using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using AutomationFoundation.Features.ProducerConsumer.Engines.Configuration;
using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions;
using AutomationFoundation.Runtime.Abstractions.Threading.Primitives;

namespace AutomationFoundation.Features.ProducerConsumer.Engines
{
    /// <summary>
    /// Provides a producer engine which uses a schedule to check the producer for work availability.
    /// </summary>
    /// <typeparam name="TItem">The type of item being produced.</typeparam>
    public class ScheduledProducerEngine<TItem> : ProducerEngine<TItem>
    {
        private readonly IProducerExecutionStrategy<TItem> executionStrategy;
        private readonly IErrorHandler errorHandler;
        private readonly IScheduler scheduler;
        private readonly ScheduledEngineOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledProducerEngine{TItem}"/> class.
        /// </summary>
        /// <param name="executionStrategy">The execution strategy to use when producing items.</param>
        /// <param name="cancellationSourceFactory">The factory for creating cancellation sources.</param>
        /// <param name="errorHandler">The error handler to use if errors within the engine.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="options">The engine configuration options.</param>
        public ScheduledProducerEngine(IProducerExecutionStrategy<TItem> executionStrategy, ICancellationSourceFactory cancellationSourceFactory, IErrorHandler errorHandler, IScheduler scheduler, ScheduledEngineOptions options)
            : base(cancellationSourceFactory)
        {
            this.executionStrategy = executionStrategy ?? throw new ArgumentNullException(nameof(executionStrategy));
            this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        protected override async Task RunAsync(Action<IProducerConsumerContext<TItem>> onProducedCallback, CancellationToken cancellationToken, CancellationToken parentToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var started = DateTimeOffset.Now;
                    var found = false;

                    do
                    {
                        try
                        {
                            found = await executionStrategy.ExecuteAsync(onProducedCallback, parentToken);
                        }
                        catch (Exception ex)
                        {
                            errorHandler.Handle(ex, ErrorSeverityLevel.NonFatal);
                        }
                    } while (found && options.ContinueUntilEmpty
                                   && !cancellationToken.IsCancellationRequested);

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await WaitUntilNextExecutionAsync(started, DateTimeOffset.Now, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                errorHandler.Handle(ex, ErrorSeverityLevel.Fatal);
            }
        }

        private async Task WaitUntilNextExecutionAsync(DateTimeOffset started, DateTimeOffset lastCompleted, CancellationToken cancellationToken)
        {
            var delay = CalculateDelayUntilNextExecution(started, lastCompleted);
            if (delay == TimeSpan.Zero)
            {
                return;
            }

            await DelayAsync(delay, cancellationToken);
        }

        /// <summary>
        /// Delays the producer.
        /// </summary>
        /// <param name="delay">The amount of time to delay the producer.</param>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        /// <returns>The task to await.</returns>
        protected virtual async Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Swallow any exceptions that indicate the task is being cancelled.
            }
        }

        private TimeSpan CalculateDelayUntilNextExecution(DateTimeOffset started, DateTimeOffset lastCompleted)
        {
            var delay = scheduler.CalculateNextExecution(started, lastCompleted) - lastCompleted;
            if (delay < TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }

            return delay;
        }
    }
}