using System;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer
{
    /// <summary>
    /// Contains contextual information regarding the consumption of work.
    /// </summary>
    /// <typeparam name="TItem">The type of item which is being consumed.</typeparam>
    public class ConsumptionContext<TItem>
    {
        /// <summary>
        /// Gets the consumer which will consume the item.
        /// </summary>
        public virtual IConsumer<TItem> Consumer { get; set; }

        /// <summary>
        /// Gets the consumption strategy for consuming the item.
        /// </summary>
        public virtual IConsumerExecutionStrategy<TItem> ExecutionStrategy { get; set; }

        /// <summary>
        /// Gets the date and time when the item was consumed.
        /// </summary>
        public virtual DateTime ConsumedOn { get; set; }

        /// <summary>
        /// Gets the duration of time taken to consume the item.
        /// </summary>
        public virtual TimeSpan? Duration { get; set; }
    }
}