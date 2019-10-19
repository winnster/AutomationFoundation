using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies an execution strategy for an <see cref="IProducer{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of item being produced.</typeparam>
    public interface IProducerExecutionStrategy<TItem>
    {
        /// <summary>
        /// Executes the strategy.
        /// </summary>
        /// <param name="onProducedCallback">The callback to execute if an object is produced.</param>
        /// <param name="parentToken">The parent cancellation token which should be monitored for cancellation requests while producing.</param>
        /// <returns>true if an item was produced, otherwise false.</returns>
        Task<bool> ExecuteAsync(Action<ProducerConsumerContext<TItem>> onProducedCallback, CancellationToken parentToken);
    }
}