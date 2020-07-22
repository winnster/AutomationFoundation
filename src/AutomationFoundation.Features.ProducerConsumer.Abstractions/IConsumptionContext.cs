using System;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies an object which provides contextual information regarding the consumption of work.
    /// </summary>
    /// <typeparam name="TItem">The type of objects being consumed.</typeparam>
    public interface IConsumptionContext<TItem>
    {
        /// <summary>
        /// Gets or sets the consumer which will consume the item.
        /// </summary>
        IConsumer<TItem> Consumer { get; set; }

        /// <summary>
        /// Gets or sets the consumption strategy for consuming the item.
        /// </summary>
        IConsumerExecutionStrategy<TItem> ExecutionStrategy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the item was consumed.
        /// </summary>
        DateTime ConsumedOn { get; set; }

        /// <summary>
        /// Gets or sets the duration of time taken to consume the item.
        /// </summary>
        TimeSpan? Duration { get; set; }
    }
}