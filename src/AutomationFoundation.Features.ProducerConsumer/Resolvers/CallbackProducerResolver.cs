using System;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer.Resolvers
{
    /// <summary>
    /// Provides a mechanism which resolves an <see cref="IProducer{TItem}"/> using a callback function.
    /// </summary>
    /// <typeparam name="TItem">The type of item being produced.</typeparam>
    public class CallbackProducerResolver<TItem> : IProducerResolver<TItem>
    {
        private readonly Func<IProducerConsumerContext<TItem>, IProducer<TItem>> callback;

        /// <summary>
        /// Initializes an instance of the <see cref="CallbackProducerResolver{TItem}"/> class.
        /// </summary>
        /// <param name="callback">The callback to execute to resolve the producer.</param>
        public CallbackProducerResolver(Func<IProducerConsumerContext<TItem>, IProducer<TItem>> callback)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        /// <inheritdoc />
        public IProducer<TItem> Resolve(IProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return callback(context);
        }

    }
}