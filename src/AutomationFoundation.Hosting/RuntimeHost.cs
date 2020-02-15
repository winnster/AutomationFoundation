using System;
using AutomationFoundation.Hosting.Abstractions;
using AutomationFoundation.Hosting.Abstractions.Builders;
using AutomationFoundation.Runtime.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutomationFoundation.Hosting
{
    /// <summary>
    /// Provides a host for the runtime.
    /// </summary>
    public class RuntimeHost : IRuntimeHost
    {
        private readonly IRuntime runtime;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc />
        public IServiceProvider ApplicationServices { get; }

        /// <inheritdoc />
        public IHostingEnvironment Environment { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeHost"/> class.
        /// </summary>
        /// <param name="runtime">The runtime to host.</param>
        /// <param name="environment">The hosting environment.</param>
        /// <param name="applicationServices">The application services available.</param>
        /// <param name="logger">The logger instance to use for logging information.</param>
        public RuntimeHost(IRuntime runtime, IHostingEnvironment environment, IServiceProvider applicationServices, ILogger<RuntimeHost> logger)
        {
            this.runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            ApplicationServices = applicationServices;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public void Start()
        {
            Logger.LogInformation(new EventId(1000, "RUNTIME_HOST_STARTING"), "Starting the host...");

            runtime.Start();

            Logger.LogInformation(new EventId(1001, "RUNTIME_HOST_STARTED"), "Started the host.");
        }

        /// <inheritdoc />
        public void Stop()
        {
            Logger.LogInformation(new EventId(1002, "RUNTIME_HOST_STOPPING"), "Stopping the host...");

            runtime.Stop();

            Logger.LogInformation(new EventId(1003, "RUNTIME_HOST_STOPPED"), "Stopped the host.");
        }

        /// <summary>
        /// Create a runtime host builder.
        /// </summary>
        /// <returns>The runtime host builder.</returns>
        public static TBuilder CreateBuilder<TBuilder>()
            where TBuilder : IRuntimeHostBuilder, new()
        {
            return new TBuilder();
        }
    }
}