using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using AutomationFoundation.Runtime;

namespace AutomationFoundation.Features.ProducerConsumer.Strategies
{
    /// <summary>
    /// Provides the default execution strategy for <see cref="IConsumer{TItem}"/> instances.
    /// </summary>
    /// <typeparam name="TItem">The type of object being consumed.</typeparam>
    public class DefaultConsumerExecutionStrategy<TItem> : IConsumerExecutionStrategy<TItem>
    {
        private readonly IConsumerResolver<TItem> consumerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConsumerExecutionStrategy{TItem}"/> class.
        /// </summary>
        /// <param name="consumerFactory">The factory for creating consumers.</param>
        public DefaultConsumerExecutionStrategy(IConsumerResolver<TItem> consumerFactory)
        {
            this.consumerFactory = consumerFactory ?? throw new ArgumentNullException(nameof(consumerFactory));
        }

        /// <inheritdoc />
        public Task ExecuteAsync(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return ExecuteAsyncImpl(context);
        }

        private async Task ExecuteAsyncImpl(IProducerConsumerContext<TItem> context)
        {
            try
            {
                OnStarted(context);

                try
                {
                    CreateConsumer(context);
                    await ConsumeAsync(context);
                }
                finally
                {
                    OnCompleted(context);
                }
            }
            finally
            {
                OnExit(context);
            }
        }

        /// <summary>
        /// Occurs when the strategy is starting to execute.
        /// </summary>
        /// <param name="context">The contextual information about the item produced.</param>
        protected virtual void OnStarted(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.ConsumptionContext.ExecutionStrategy = this;
            ProcessingContext.SetCurrent(context);
        }

        /// <summary>
        /// Occurs when the strategy has completed execution.
        /// </summary>
        /// <param name="context">The contextual information about the item produced.</param>
        protected virtual void OnCompleted(IProducerConsumerContext<TItem> context)
        {
            ProcessingContext.Clear();
        }

        /// <summary>
        /// Occurs when the strategy is exiting.
        /// </summary>
        /// <param name="context">The contextual information about the item produced.</param>
        protected virtual void OnExit(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Dispose();
        }

        /// <summary>
        /// Creates the consumer which will consume the object.
        /// </summary>
        /// <param name="context">The contextual information about the item being consumed.</param>
        protected virtual void CreateConsumer(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var consumer = consumerFactory.Resolve(context);
            if (consumer == null)
            {
                throw new InvalidOperationException("The consumer was not created.");
            }

            context.ConsumptionContext.Consumer = consumer;
        }

        /// <summary>
        /// Consumes the item.
        /// </summary>
        /// <param name="context">The contextual information about the item being consumed.</param>
        protected virtual async Task ConsumeAsync(IProducerConsumerContext<TItem> context)
        {
            context.ConsumptionContext.ConsumedOn = DateTime.Now;

            var stopwatch = Stopwatch.StartNew();
            await context.ConsumptionContext.Consumer.ConsumeAsync(context);
            stopwatch.Stop();

            context.ConsumptionContext.Duration = stopwatch.Elapsed;
        }
    }
}