using System.Threading.Tasks;

namespace AutomationFoundation.Runtime.Abstractions
{
    /// <summary>
    /// Identifies an engine.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Starts the engine asynchronously.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        Task StartAsync();

        /// <summary>
        /// Stops the engine asynchronously.
        /// </summary>
        /// <returns>The task to await.</returns>
        Task StopAsync();

        /// <summary>
        /// Waits for the engine to stop.
        /// </summary>
        /// <returns>The task to await.</returns>
        Task WaitForCompletionAsync();
    }
}