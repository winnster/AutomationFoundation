using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions.Synchronization;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies an object which provides contextual information regarding the production and consumption of work being processed by the runtime.
    /// </summary>
    /// <typeparam name="TItem">The type of item which was produced.</typeparam>
    public interface IProducerConsumerContext<TItem> : IProcessingContext
    {
        /// <summary>
        /// Gets or sets the item which was produced.
        /// </summary>
        TItem Item { get; set; }

        /// <summary>
        /// Gets or sets the synchronization lock for the produced item.
        /// </summary>
        ISynchronizationLock SynchronizationLock { get; set; }

        /// <summary>
        /// Gets the contextual information regarding the production of work.
        /// </summary>
        IProductionContext<TItem> ProductionContext { get; }

        /// <summary>
        /// Gets the contextual information regarding the consumption of work.
        /// </summary>
        IConsumptionContext<TItem> ConsumptionContext { get; }
    }
}