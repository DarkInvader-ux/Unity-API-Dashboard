using System;
using UnityEngine;

namespace Core.Configuration
{
    /// <summary>
    /// Configuration settings for the Events API
    /// </summary>
    [Serializable]
    public class ApiConfiguration
    {
        [Header("API Settings")]
        [SerializeField] private string baseUrl = "http://localhost:5089";
        [SerializeField] private string eventsEndpoint = "/Events";
        [SerializeField] private int timeoutSeconds = 30;

        /// <summary>
        /// Base URL for the API
        /// </summary>
        public string BaseUrl => baseUrl;
        
        /// <summary>
        /// Events endpoint path
        /// </summary>
        public string EventsEndpoint => eventsEndpoint;
        
        /// <summary>
        /// Complete URL for events endpoint
        /// </summary>
        public string EventsUrl => baseUrl + eventsEndpoint;
        
        /// <summary>
        /// Request timeout in seconds
        /// </summary>
        public int TimeoutSeconds => timeoutSeconds;

        /// <summary>
        /// Updates the base URL at runtime
        /// </summary>
        /// <param name="url">New base URL</param>
        public void SetBaseUrl(string url) => baseUrl = url;
        
        /// <summary>
        /// Updates the events endpoint at runtime
        /// </summary>
        /// <param name="endpoint">New endpoint path</param>
        public void SetEventsEndpoint(string endpoint) => eventsEndpoint = endpoint;
        
        /// <summary>
        /// Updates the timeout setting at runtime
        /// </summary>
        /// <param name="seconds">New timeout in seconds</param>
        public void SetTimeout(int seconds) => timeoutSeconds = seconds;
    }
}