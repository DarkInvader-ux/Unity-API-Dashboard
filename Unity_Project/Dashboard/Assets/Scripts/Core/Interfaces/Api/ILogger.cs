namespace Core.Interfaces.Api
{
    /// <summary>
    /// Abstraction for logging operations
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Log(string message);
        
        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Error message to log</param>
        void LogError(string message);
        
        /// <summary>
        /// Logs a success message
        /// </summary>
        /// <param name="message">Success message to log</param>
        void LogSuccess(string message);
    }
}