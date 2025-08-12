using UnityEngine;
using ILogger = Core.Interfaces.ILogger;

namespace Core.Services.Events
{
    /// <summary>
    /// Unity console implementation of ILogger
    /// </summary>
    public class UnityConsoleLogger : ILogger
    {
        private readonly string prefix;
        private readonly bool enableLogging;
        private ILogger _loggerImplementation;

        /// <summary>
        /// Creates a new Unity console logger
        /// </summary>
        /// <param name="logPrefix">Prefix for all log messages</param>
        /// <param name="enabled">Whether logging is enabled</param>
        public UnityConsoleLogger(string logPrefix = "[API]", bool enabled = true)
        {
            prefix = logPrefix;
            enableLogging = enabled;
        }

        /// <summary>
        /// Logs an informational message to Unity console
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Log(string message)
        {
            if (enableLogging) 
                Debug.Log($"{prefix} {message}");
        }

        /// <summary>
        /// Logs an error message to Unity console
        /// </summary>
        /// <param name="message">Error message to log</param>
        public void LogError(string message)
        {
            if (enableLogging) 
                Debug.LogError($"{prefix} ❌ {message}");
        }

        /// <summary>
        /// Logs a success message to Unity console
        /// </summary>
        /// <param name="message">Success message to log</param>
        public void LogSuccess(string message)
        {
            if (enableLogging) 
                Debug.Log($"{prefix} ✅ {message}");
        }
    }
}