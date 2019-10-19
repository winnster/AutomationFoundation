using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;
using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions.Threading.Primitives;

namespace AutomationFoundation.Features.ProducerConsumer
{
    /// <summary>
    /// Provides a processor implementation for the Producer/Consumer pattern.
    /// </summary>
    /// <typeparam name="TItem">The type of item to be produced and consumed.</typeparam>
    public class ProducerConsumerProcessor<TItem> : Processor
    {
        private readonly IEnumerable<IProducerEngine<TItem>> producerEngines;
        private readonly ICancellationSourceFactory cancellationSourceFactory;
        private readonly IConsumerEngine<TItem> consumerEngine;

        private ICancellationSource cancellationSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerConsumerProcessor{TItem}"/> class.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <param name="cancellationSourceFactory">The factory for creating cancellation sources.</param>
        /// <param name="producerEngines">The engines which will produce objects to process.</param>
        /// <param name="consumerEngine">The consumer engine which will consume objects which were produced.</param>
        public ProducerConsumerProcessor(string name, ICancellationSourceFactory cancellationSourceFactory, IEnumerable<IProducerEngine<TItem>> producerEngines, IConsumerEngine<TItem> consumerEngine)
            : base(name)
        {
            this.cancellationSourceFactory = cancellationSourceFactory ?? throw new ArgumentNullException(nameof(cancellationSourceFactory));
            this.producerEngines = producerEngines ?? throw new ArgumentNullException(nameof(producerEngines));
            this.consumerEngine = consumerEngine ?? throw new ArgumentNullException(nameof(consumerEngine));
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            InitializeCancellationSource();

            InitializeProducerEngines();
            InitializeConsumerEngine();

            using (var t1 = StartProducerEngines())
            using (var t2 = StartConsumerEngine())
            {
                Task.WaitAll(t1, t2);
            }
        }

        private void InitializeCancellationSource()
        {
            cancellationSource?.Dispose();

            cancellationSource = cancellationSourceFactory.Create(CancellationToken.None);
            if (cancellationSource == null)
            {
                throw new InvalidOperationException("The cancellation source factory did not produce a cancellation source.");
            }
        }

        private void InitializeProducerEngines()
        {
            foreach (var producerEngine in producerEngines)
            {
                producerEngine.Initialize(
                    OnProducedCallback,
                    cancellationSource.CancellationToken);
            }
        }

        private void InitializeConsumerEngine()
        {
            consumerEngine.Initialize(cancellationSource.CancellationToken);
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            using (var t1 = StopProducerEngines())
            using (var t2 = StopConsumerEngine())
            {
                Task.WaitAll(t1, t2);
            }
        }

        private Task StartProducerEngines()
        {
            var tasks = new List<Task>();

            foreach (var producerEngine in producerEngines)
            {
                var task = producerEngine.StartAsync();
                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }

        private void OnProducedCallback(ProducerConsumerContext<TItem> context)
        {
            context.Processor = this;

            consumerEngine.Consume(context);
        }

        private Task StopProducerEngines()
        {
            var tasks = new List<Task>();

            foreach (var producerEngine in producerEngines)
            {
                var task = producerEngine.StopAsync();

                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }

        private Task StartConsumerEngine()
        {
            return consumerEngine.StartIfAsynchronous();
        }

        private Task StopConsumerEngine()
        {
            return consumerEngine.StopIfAsynchronous();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                cancellationSource?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}