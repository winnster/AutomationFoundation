using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace AutomationFoundation.Features.ProducerConsumer.Strategies
{
    /// <summary>
    /// Provides the default execution strategy for <see cref="IProducer{TItem}"/> instances.
    /// </summary>
    /// <typeparam name="TItem">The type of item being produced.</typeparam>
    public class DefaultProducerExecutionStrategy<TItem> : IProducerExecutionStrategy<TItem>
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ISynchronizationPolicy synchronizationPolicy;
        private readonly IProducerResolver<TItem> producerFactory;
        private readonly bool alwaysExecuteOnDefaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProducerExecutionStrategy{TItem}"/> class.
        /// </summary>
        /// <param name="scopeFactory">The factory for creating service scopes.</param>
        /// <param name="producerFactory">The factory for creating producers.</param>
        /// <param name="synchronizationPolicy">The policy used to synchronize the producer and consumer engines to prevent over producing work.</param>
        /// <param name="alwaysExecuteOnDefaultValue">true to always execute the callback, even if the value produced is the default; otherwise false.</param>
        public DefaultProducerExecutionStrategy(IServiceScopeFactory scopeFactory, IProducerResolver<TItem> producerFactory, ISynchronizationPolicy synchronizationPolicy, bool alwaysExecuteOnDefaultValue)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.producerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
            this.synchronizationPolicy = synchronizationPolicy;
            this.alwaysExecuteOnDefaultValue = alwaysExecuteOnDefaultValue;
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(Action<IProducerConsumerContext<TItem>> onProducedCallback, CancellationToken parentToken)
        {
            if (onProducedCallback == null)
            {
                throw new ArgumentNullException(nameof(onProducedCallback));
            }

            return RunAsyncImpl(onProducedCallback, parentToken);
        }

        private async Task<bool> RunAsyncImpl(Action<IProducerConsumerContext<TItem>> onProducedCallback, CancellationToken parentToken)
        {
            IServiceScope scope = null;

            try
            {
                scope = CreateChildScope();

                ProducerConsumerContext<TItem> context = null;
                var called = false;

                try
                {
                    var id = GenerateIdentifier(scope);

                    context = new ProducerConsumerContext<TItem>(id, scope)
                    {
                        CancellationToken = parentToken,
                        ProductionContext =
                        {
                            ExecutionStrategy = this
                        }
                    };

                    await AcquireSynchronizationLockAsync(context);
                    await CreateProducerAsync(context);
                    await ProduceAsync(context);

                    if (ShouldExecuteCallback(context))
                    {
                        onProducedCallback(context);
                        called = true;
                    }
                }
                finally
                {
                    if (!called)
                    {
                        context?.Dispose();
                    }
                }

                return called;
            }
            catch (Exception)
            {
                scope?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Acquires a synchronization lock for the item to be produced.
        /// </summary>
        /// <param name="context">The contextual information for the item which was produced.</param>
        /// <returns>The synchronization lock.</returns>
        protected virtual Task AcquireSynchronizationLockAsync(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return AcquireSynchronizationLockAsyncImpl(context);
        }

        private async Task AcquireSynchronizationLockAsyncImpl(IProducerConsumerContext<TItem> context)
        {
            if (synchronizationPolicy == null)
            {
                return;
            }

            context.SynchronizationLock = await synchronizationPolicy.AcquireLockAsync(context.CancellationToken);
        }

        /// <summary>
        /// Creates a producer.
        /// </summary>
        /// <param name="context">The contextual information for the item which was produced.</param>
        protected virtual Task CreateProducerAsync(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var producer = producerFactory.Resolve(context);
            if (producer == null)
            {
                throw new RuntimeException("The producer was not created.");
            }

            context.ProductionContext.Producer = producer;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Produces an item.
        /// </summary>
        /// <param name="context">The contextual information for the item which was produced.</param>
        /// <returns>The task to await.</returns>
        protected virtual Task ProduceAsync(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return ProduceAsyncImpl(context);
        }

        private async Task ProduceAsyncImpl(IProducerConsumerContext<TItem> context)
        {
            context.ProductionContext.ProducedOn = DateTime.Now;

            var stopwatch = Stopwatch.StartNew();
            context.Item = await context.ProductionContext.Producer.ProduceAsync(context.CancellationToken);
            stopwatch.Stop();

            context.ProductionContext.Duration = stopwatch.Elapsed;
        }

        /// <summary>
        /// Creates the child scope.
        /// </summary>
        /// <returns>The child scope.</returns>
        protected virtual IServiceScope CreateChildScope()
        {
            var scope = scopeFactory.CreateScope();
            if (scope == null)
            {
                throw new RuntimeException("The scope factory did not return a scope.");
            }

            return scope;
        }

        /// <summary>
        /// Generates a new identifier.
        /// </summary>
        /// <param name="scope">The scope to use when creating the identifier.</param>
        /// <returns>The new identifier.</returns>
        protected virtual Guid GenerateIdentifier(IServiceScope scope)
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Determines whether the callback should be executed.
        /// </summary>
        /// <param name="context">The contextual information for the item which was produced.</param>
        /// <returns>true if the callback should be executed, otherwise false.</returns>
        protected virtual bool ShouldExecuteCallback(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return alwaysExecuteOnDefaultValue || !Equals(context.Item, default(TItem));
        }
    }
}