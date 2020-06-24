﻿using System.Threading;
using System.Threading.Tasks;
using AutomationFoundation.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AutomationFoundation
{
    /// <summary>
    /// Contains extensions for the runtime host.
    /// </summary>
    public static class RuntimeHostExtensions
    {
        /// <summary>
        /// Defines the default timeout (in milliseconds) of the startup and shutdown procedures.
        /// </summary>
        private const int DefaultTimeoutMs = Timeout.Infinite;

        /// <summary>
        /// Runs until signaled to stop via the run strategy.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="startupTimeoutMs">The timeout (in milliseconds) upon which startup will be aborted.</param>
        /// <param name="shutdownTimeoutMs">The timeout (in milliseconds) upon which shutdown will no longer be graceful, and shall be forced.</param>
        /// <returns>The task to await.</returns>
        public static async Task RunAsync(this IRuntimeHost host, int startupTimeoutMs = DefaultTimeoutMs, int shutdownTimeoutMs = DefaultTimeoutMs)
        {
            var runStrategy = host.ApplicationServices.GetService<IRuntimeHostRunAsyncStrategy>();
            if (runStrategy == null)
            {
                throw new HostingException($"The run strategy has not been defined. Please ensure the {nameof(IRuntimeHostBuilder.UseRunStrategy)} method has been called on the {nameof(IRuntimeHostBuilder)}.");
            }

            await runStrategy.RunAsync(host, startupTimeoutMs, shutdownTimeoutMs);
        }
    }
}