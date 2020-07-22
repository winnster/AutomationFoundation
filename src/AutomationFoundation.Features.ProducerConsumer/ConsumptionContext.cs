using System;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer
{
    /// <summary>
    /// Contains contextual information regarding the consumption of work.
    /// </summary>
    /// <typeparam name="TItem">The type of item which is being consumed.</typeparam>
    public class ConsumptionContext<TItem> : IConsumptionContext<TItem>
    {
        /// <inheritdoc />
        public IConsumer<TItem> Consumer { get; set; }

        /// <inheritdoc />
        public IConsumerExecutionStrategy<TItem> ExecutionStrategy { get; set; }

        /// <inheritdoc />
        public DateTime ConsumedOn { get; set; }

        /// <inheritdoc />
        public TimeSpan? Duration { get; set; }
    }
}