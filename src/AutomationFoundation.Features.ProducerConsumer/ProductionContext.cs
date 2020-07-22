using System;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer
{
    /// <summary>
    /// Contains contextual information regarding the production of work.
    /// </summary>
    /// <typeparam name="TItem">The type of item which was produced.</typeparam>
    public class ProductionContext<TItem> : IProductionContext<TItem>
    {
        /// <inheritdoc />
        public IProducer<TItem> Producer { get; set; }

        /// <inheritdoc />
        public IProducerExecutionStrategy<TItem> ExecutionStrategy { get; set; }

        /// <inheritdoc />
        public DateTime ProducedOn { get; set; }

        /// <inheritdoc />
        public TimeSpan? Duration { get; set; }
    }
}