using System;
using System.Threading.Tasks;
using AutomationFoundation.Features.ProducerConsumer.Abstractions;

namespace AutomationFoundation.Features.ProducerConsumer.Strategies.TestObjects
{
    public class StubDefaultConsumerExecutionStrategy<TItem> : DefaultConsumerExecutionStrategy<TItem>
    {
        private readonly Action<StubDefaultConsumerExecutionStrategy<TItem>, ProducerConsumerContext<TItem>> onStartedCallback;
        private readonly Action<StubDefaultConsumerExecutionStrategy<TItem>, ProducerConsumerContext<TItem>> onConsumeCallback;
        private readonly Action<StubDefaultConsumerExecutionStrategy<TItem>, ProducerConsumerContext<TItem>> onCompletedCallback;
        private readonly Action<StubDefaultConsumerExecutionStrategy<TItem>, ProducerConsumerContext<TItem>> onExitCallback;

        private ProducerConsumerContext<TItem> overrideContext;
        private bool isContextOverridden;

        public StubDefaultConsumerExecutionStrategy(IConsumerResolver<TItem> consumerFactory, Action<StubDefaultConsumerExecutionStrategy<TItem>, ProducerConsumerContext<TItem>> onStartedCallback = null, Action<StubDefaultConsumerExecutionStrategy<TItem>, ProducerConsumerContext<TItem>> onConsumeCallback = null,
            Action<StubDefaultConsumerExecutionStrategy<TItem>, ProducerConsumerContext<TItem>> onCompletedCallback = null,
            Action<StubDefaultConsumerExecutionStrategy<TItem>, ProducerConsumerContext<TItem>> onExitCallback = null)
            : base(consumerFactory)
        {
            this.onStartedCallback = onStartedCallback;
            this.onConsumeCallback = onConsumeCallback;
            this.onCompletedCallback = onCompletedCallback;
            this.onExitCallback = onExitCallback;
        }

        public void SetOverrideContext(ProducerConsumerContext<TItem> context)
        {
            overrideContext = context;
            isContextOverridden = true;
        }

        private ProducerConsumerContext<TItem> GetContext(ProducerConsumerContext<TItem> context)
        {
            if (isContextOverridden)
            {
                return overrideContext;
            }

            return context;
        }

        protected override void OnStarted(ProducerConsumerContext<TItem> context)
        {
            var c = GetContext(context);

            base.OnStarted(c);
            onStartedCallback?.Invoke(this, c);
        }

        protected override void CreateConsumer(ProducerConsumerContext<TItem> context)
        {
            var c = GetContext(context);

            base.CreateConsumer(c);
        }

        protected override async Task ConsumeAsync(ProducerConsumerContext<TItem> context)
        {
            var c = GetContext(context);

            await base.ConsumeAsync(c);
            onConsumeCallback?.Invoke(this, c);
        }

        protected override void OnCompleted(ProducerConsumerContext<TItem> context)
        {
            var c = GetContext(context);

            base.OnCompleted(c);
            onCompletedCallback?.Invoke(this, c);
        }

        protected override void OnExit(ProducerConsumerContext<TItem> context)
        {
            var c = GetContext(context);

            base.OnExit(c);
            onExitCallback?.Invoke(this, c);
        }
    }
}