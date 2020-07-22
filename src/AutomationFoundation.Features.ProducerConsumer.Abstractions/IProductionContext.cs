using System;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies an object which provides contextual information regarding the production of work.
    /// </summary>
    /// <typeparam name="TItem">The type of objects being produced.</typeparam>
    public interface IProductionContext<TItem>
    {
        /// <summary>
        /// Gets or sets the producer instance which produced the item.
        /// </summary>
        IProducer<TItem> Producer { get; set; }

        /// <summary>
        /// Gets or sets the production strategy for producing the item.
        /// </summary>
        IProducerExecutionStrategy<TItem> ExecutionStrategy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the item was produced.
        /// </summary>
        DateTime ProducedOn { get; set; }

        /// <summary>
        /// Gets or sets the duration of time taken to produce the item.
        /// </summary>
        TimeSpan? Duration { get; set; }
    }
}