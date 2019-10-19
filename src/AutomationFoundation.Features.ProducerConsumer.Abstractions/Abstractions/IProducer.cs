using System.Threading;
using System.Threading.Tasks;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies a producer of objects.
    /// </summary>
    /// <typeparam name="TItem">The type of objects which will be produced.</typeparam>
    public interface IProducer<TItem>
    {
        /// <summary>
        /// Produces an object asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        /// <returns>The task which will return the object produced.</returns>
        Task<TItem> ProduceAsync(CancellationToken cancellationToken);
    }
}