using System.Threading;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies a consumer engine.
    /// </summary>
    /// <typeparam name="TItem">The type of item being consumed.</typeparam>
    public interface IConsumerEngine<TItem>
    {
        /// <summary>
        /// Initializes the engine.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        void Initialize(CancellationToken cancellationToken);

        /// <summary>
        /// Consumes the item.
        /// </summary>
        /// <param name="context">The object containing contextual information about an item which was produced.</param>
        void Consume(ProducerConsumerContext<TItem> context);
    }
}