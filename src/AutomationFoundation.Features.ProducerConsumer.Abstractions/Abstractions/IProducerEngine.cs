using System;
using System.Threading;
using AutomationFoundation.Runtime.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies a producer engine.
    /// </summary>
    /// <typeparam name="TItem">The type of item being produced.</typeparam>
    public interface IProducerEngine<TItem> : IStartable, IStoppable
    {
        /// <summary>
        /// Initializes the engine.
        /// </summary>
        /// <param name="onProducedCallback">The callback to execute when an item is produced.</param>
        /// <param name="parentToken">The parent cancellation token.</param>
        void Initialize(Action<ProducerConsumerContext<TItem>> onProducedCallback, CancellationToken parentToken);
    }
}