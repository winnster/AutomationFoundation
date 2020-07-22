namespace AutomationFoundation.Features.ProducerConsumer.Engines.Configuration
{
    /// <summary>
    /// Contains configuration options for the scheduled engine.
    /// </summary>
    public class ScheduledEngineOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the engine should continue processing until no data is returned.
        /// </summary>
        public bool ContinueUntilEmpty { get; set; }
    }
}