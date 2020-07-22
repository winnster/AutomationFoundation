namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Identifies an object which can create an <see cref="IProducer{TItem}"/>
    /// </summary>
    /// <typeparam name="TItem">The type of item being produced.</typeparam>
    public interface IProducerResolver<TItem>
    {
        /// <summary>
        /// Resolves the producer.
        /// </summary>
        /// <param name="context">The contextual information for the item being processed.</param>
        /// <returns>The producer instance.</returns>
        IProducer<TItem> Resolve(IProducerConsumerContext<TItem> context);
    }
}