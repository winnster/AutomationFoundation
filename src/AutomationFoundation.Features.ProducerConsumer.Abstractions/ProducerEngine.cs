using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationFoundation.Runtime;
using AutomationFoundation.Runtime.Abstractions.Threading.Primitives;

namespace AutomationFoundation.Features.ProducerConsumer.Abstractions
{
    /// <summary>
    /// Provides a base implementation of a producer engine. This class must be inherited.
    /// </summary>
    /// <typeparam name="TItem">The type of item being produced.</typeparam>
    public abstract class ProducerEngine<TItem> : Engine, IProducerEngine<TItem>
    {
        private readonly ICancellationSourceFactory cancellationSourceFactory;

        private Action<IProducerConsumerContext<TItem>> onProducedCallback;
        private ICancellationSource cancellationSource;
        private CancellationToken parentToken;
        private bool initialized;
        private Task task;

        /// <summary>
        /// Gets a value indicating whether the engine is currently running.
        /// </summary>
        public bool IsRunning => task != null && !task.IsCanceled && !task.IsCompleted && !task.IsFaulted;

        /// <summary>
        /// Gets a value indicating whether the engine has been started.
        /// </summary>
        public bool IsStarted => task != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerEngine{TItem}"/> class.
        /// </summary>
        /// <param name="cancellationSourceFactory">The factory for creating cancellation sources.</param>
        protected ProducerEngine(ICancellationSourceFactory cancellationSourceFactory)
        {
            this.cancellationSourceFactory = cancellationSourceFactory ?? throw new ArgumentNullException(nameof(cancellationSourceFactory));
        }

        /// <inheritdoc />
        public void Initialize(Action<IProducerConsumerContext<TItem>> onProducedCallback, CancellationToken parentToken)
        {
            if (onProducedCallback == null)
            {
                throw new ArgumentNullException(nameof(onProducedCallback));
            }

            GuardMustNotBeDisposed();
            GuardMustNotBeInitialized();

            ConfigureCancellationSource(parentToken);
            this.parentToken = parentToken;
            this.onProducedCallback = onProducedCallback;
            OnInitialized();
        }

        /// <summary>
        /// Configures the cancellation source.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to use for cancellation requests.</param>
        protected virtual void ConfigureCancellationSource(CancellationToken cancellationToken)
        {
            cancellationSource?.Dispose();

            cancellationSource = cancellationSourceFactory.Create(cancellationToken);
            if (cancellationSource == null)
            {
                throw new InvalidOperationException("The cancellation source factory did not create a cancellation source.");
            }
        }

        /// <summary>
        /// Occurs after the engine has been initialized.
        /// </summary>
        protected virtual void OnInitialized()
        {
            initialized = true;
        }

        /// <summary>
        /// Ensures the engine has not been previously initialized.
        /// </summary>
        /// <exception cref="EngineException">The engine has already been initialized.</exception>
        protected void GuardMustNotBeInitialized()
        {
            if (initialized)
            {
                throw new EngineException("The engine has already been initialized.");
            }
        }

        /// <inheritdoc />
        public Task StartAsync()
        {
            GuardMustNotBeDisposed();
            GuardMustBeInitialized();

            task = new Task(async () => await RunAsync(onProducedCallback, cancellationSource.CancellationToken, parentToken), TaskCreationOptions.LongRunning);
            task.Start();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Ensures the engine has been initialized.
        /// </summary>
        /// <exception cref="EngineException">The engine has not been initialized.</exception>
        private void GuardMustBeInitialized()
        {
            if (!initialized)
            {
                throw new EngineException("The engine has not been initialized.");
            }
        }

        /// <summary>
        /// Wait for the engine to complete asynchronously.
        /// </summary>
        /// <returns>The task to await.</returns>
        public Task WaitForCompletionAsync()
        {
            GuardMustNotBeDisposed();
            GuardMustBeInitialized();

            GuardMustBeStarted();

            return Task.WhenAll(task);
        }

        /// <summary>
        /// Ensures the engine has been started.
        /// </summary>
        protected void GuardMustBeStarted()
        {
            if (!IsStarted)
            {
                throw new EngineException("The engine must be started.");
            }
        }

        /// <summary>
        /// Ensures the engine is running.
        /// </summary>
        protected void GuardMustBeRunning()
        {
            if (!IsRunning)
            {
                throw new EngineException("The engine must be running.");
            }
        }

        /// <summary>
        /// Runs the engine.
        /// </summary>
        /// <param name="onProducedCallback">The callback to execute when the engine produces an item.</param>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        /// <param name="parentToken">The parent cancellation token which should be attached to the produced objects.</param>
        /// <returns>The task to await.</returns>
        protected abstract Task RunAsync(Action<IProducerConsumerContext<TItem>> onProducedCallback, CancellationToken cancellationToken, CancellationToken parentToken);

        /// <inheritdoc />
        public Task StopAsync()
        {
            GuardMustNotBeDisposed();
            GuardMustBeInitialized();

            GuardMustBeStarted();

            cancellationSource.RequestImmediateCancellation();

            return Task.WhenAll(task);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                cancellationSource?.Dispose();
                task?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}