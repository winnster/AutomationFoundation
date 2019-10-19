using System.Threading.Tasks;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies a consumer of objects.
    /// </summary>
    /// <typeparam name="TItem">The type of objects being consumed.</typeparam>
    public interface IConsumer<TItem>
    {
        /// <summary>
        /// Consumes the object.
        /// </summary>
        /// <param name="context">The contextual information for the item being consumed.</param>
        /// <returns>The task to await for the consumption to complete.</returns>
        Task ConsumeAsync(ProducerConsumerContext<TItem> context);
    }
}