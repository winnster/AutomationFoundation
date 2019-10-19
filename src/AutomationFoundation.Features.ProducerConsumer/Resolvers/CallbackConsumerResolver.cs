using System;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer.Resolvers
{
    /// <summary>
    /// Provides a mechanism which resolves an <see cref="IConsumer{TItem}"/> using a callback function.
    /// </summary>
    /// <typeparam name="TItem">The type of item to be consumed.</typeparam>
    public class CallbackConsumerResolver<TItem> : IConsumerResolver<TItem>
    {
        private readonly Func<ProducerConsumerContext<TItem>, IConsumer<TItem>> callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackConsumerResolver{TItem}"/> class.
        /// </summary>
        /// <param name="callback">The callback to execute to resolve the consumer.</param>
        public CallbackConsumerResolver(Func<ProducerConsumerContext<TItem>, IConsumer<TItem>> callback)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        /// <inheritdoc />
        public IConsumer<TItem> Resolve(ProducerConsumerContext<TItem> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return callback(context);
        }
    }
}