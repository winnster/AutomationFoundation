using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions.Threading;
using AutomationFoundation.Runtime.Abstractions.Threading.Primitives;

namespace AutomationFoundation.Features.ProducerConsumer.Engines
{
    /// <summary>
    /// Provides a consumer engine which consumes objects synchronously.
    /// </summary>
    /// <typeparam name="TItem">The type of item being consumed.</typeparam>
    public class SynchronousConsumerEngine<TItem> : Engine, IConsumerEngine<TItem>
    {
        private readonly IWorkerPool pool;
        private readonly IConsumerExecutionStrategy<TItem> runner;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousConsumerEngine{TItem}"/> class.
        /// </summary>
        /// <param name="pool">The pool of workers available to consume the objects.</param>
        /// <param name="runner">The consumer runner which will consume the objects produced.</param>
        public SynchronousConsumerEngine(IWorkerPool pool, IConsumerExecutionStrategy<TItem> runner)
        {
            this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        /// <inheritdoc />
        public void Initialize(CancellationToken cancellationToken)
        {
            // This method intentionally left blank.
        }

        /// <inheritdoc />
        public void Consume(ProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            using (var worker = CreateWorker(context))
            {
                worker.Run();
            }
        }

        /// <summary>
        /// Creates a new worker.
        /// </summary>
        /// <param name="context">The contextual information about what was produced.</param>
        /// <returns>The worker used to work the produced item.</returns>
        protected virtual IWorker CreateWorker(ProducerConsumerContext<TItem> context)
        {
            return pool.Get(() => OnConsume(context), null);
        }

        /// <summary>
        /// Occurs when an item is being consumed.
        /// </summary>
        /// <param name="context">The contextual information about what was produced.</param>
        protected virtual void OnConsume(ProducerConsumerContext<TItem> context)
        {
            using (var task = runner.ExecuteAsync(context))
            {
                Task.WaitAll(task);
            }
        }
    }
}