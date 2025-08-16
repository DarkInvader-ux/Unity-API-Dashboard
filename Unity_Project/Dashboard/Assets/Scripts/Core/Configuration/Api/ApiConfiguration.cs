using System;
using UnityEngine;

namespace Core.Configuration.Api
{
    [Serializable]
    public class ApiConfiguration
    {
        [Header("API Settings")]
        [SerializeField] private string baseUrl = "http://localhost:5089";
        [SerializeField] private string eventsEndpoint = "/Events";
        [SerializeField] private int timeoutSeconds = 30;
        
        public string BaseUrl => baseUrl;

        public string EventsEndpoint => eventsEndpoint;

        public string EventsUrl => baseUrl + eventsEndpoint;

        public int TimeoutSeconds => timeoutSeconds;
        
        public void SetBaseUrl(string url) => baseUrl = url;
        
        public void SetEventsEndpoint(string endpoint) => eventsEndpoint = endpoint;

        public void SetTimeout(int seconds) => timeoutSeconds = seconds;
    }
}