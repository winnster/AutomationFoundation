using System.Threading.Tasks;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies an execution strategy for an <see cref="IConsumer{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of item being consumed.</typeparam>
    public interface IConsumerExecutionStrategy<TItem>
    {
        /// <summary>
        /// Executes the strategy.
        /// </summary>
        /// <param name="context">The contextual information about the item produced.</param>
        Task ExecuteAsync(ProducerConsumerContext<TItem> context);
    }
}