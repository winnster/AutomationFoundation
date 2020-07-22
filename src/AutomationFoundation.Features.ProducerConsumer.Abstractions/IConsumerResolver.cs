namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies an object which can create an <see cref="IConsumer{TItem}"/>
    /// </summary>
    /// <typeparam name="TItem">The type of item being consumed.</typeparam>
    public interface IConsumerResolver<TItem>
    {
        /// <summary>
        /// Resolves the consumer.
        /// </summary>
        /// <param name="context">The contextual information for the item being processed.</param>
        /// <returns>The consumer instance.</returns>
        IConsumer<TItem> Resolve(IProducerConsumerContext<TItem> context);
    }
}