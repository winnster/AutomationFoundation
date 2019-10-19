using System;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer
{
    /// <summary>
    /// Contains contextual information regarding the production of work.
    /// </summary>
    /// <typeparam name="TItem">The type of item which was produced.</typeparam>
    public class ProductionContext<TItem>
    {
        /// <summary>
        /// Gets or sets the producer instance which produced the item.
        /// </summary>
        public virtual IProducer<TItem> Producer { get; set; }

        /// <summary>
        /// Gets or sets the execution strategy for producing the item.
        /// </summary>
        public virtual IProducerExecutionStrategy<TItem> ExecutionStrategy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the item was produced.
        /// </summary>
        public virtual DateTime ProducedOn { get; set; }

        /// <summary>
        /// Gets or sets the duration of time taken to produce the item.
        /// </summary>
        public virtual TimeSpan? Duration { get; set; }
    }
}