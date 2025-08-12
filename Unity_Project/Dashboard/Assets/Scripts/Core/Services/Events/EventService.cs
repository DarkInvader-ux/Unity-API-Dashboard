using System;
using System.Collections;
using Core.Configuration;
using Core.Interfaces;
using Core.Models;
using UnityEngine;
using ILogger = Core.Interfaces.ILogger;

namespace Core.Services.Events
{
    public class EventsService : IEventsService
    {
        private readonly IHttpClient httpClient;
        private readonly ApiConfiguration config;
        private readonly ILogger logger;

        /// <summary>
        /// Creates a new Events service
        /// </summary>
        /// <param name="httpClient">HTTP client for making requests</param>
        /// <param name="config">API configuration</param>
        /// <param name="logger">Logger for service operations</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
        public EventsService(IHttpClient httpClient, ApiConfiguration config, ILogger logger)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Posts an event to the API
        /// </summary>
        /// <param name="eventName">Name of the event to post</param>
        /// <returns>Coroutine for async execution</returns>
        public IEnumerator PostEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                logger.LogError("Event name cannot be null or empty");
                yield break;
            }

            var request = new HttpRequest<string>
            {
                Url = config.EventsUrl,
                Method = HttpMethod.POST,
                Data = eventName,
                OnComplete = response => OnEventPosted(eventName, response),
                OnError = error => logger.LogError($"Failed to post event '{eventName}': {error}")
            };

            yield return httpClient.SendRequest(request);
        }

        /// <summary>
        /// Retrieves all events from the API
        /// </summary>
        /// <returns>Coroutine for async execution</returns>
        public IEnumerator GetEvents()
        {
            var request = new HttpRequest<object>
            {
                Url = config.EventsUrl,
                Method = HttpMethod.GET,
                Data = null,
                OnComplete = OnEventsReceived,
                OnError = error => logger.LogError($"Failed to get events: {error}")
            };

            yield return httpClient.SendRequest(request);
        }

        /// <summary>
        /// Handles successful event posting
        /// </summary>
        private void OnEventPosted(string eventName, HttpResponse response)
        {
            logger.LogSuccess($"Event '{eventName}' posted successfully");
            logger.Log($"Server response: {response.ResponseText}");
        }

        /// <summary>
        /// Handles successful events retrieval and parsing
        /// </summary>
        private void OnEventsReceived(HttpResponse response)
        {
            try
            {
                // Wrap the response in an object since JsonUtility needs an object wrapper
                var wrappedJson = $"{{\"events\":{response.ResponseText}}}";
                var eventsResponse = JsonUtility.FromJson<EventsResponse>(wrappedJson);
                
                if (eventsResponse?.events != null)
                {
                    logger.LogSuccess($"Retrieved {eventsResponse.events.Length} events:");
                    for (int i = 0; i < eventsResponse.events.Length; i++)
                    {
                        logger.Log($"  [{i}] {eventsResponse.events[i]}");
                    }
                }
                else
                {
                    logger.Log("No events found or empty response");
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to parse events response: {e.Message}");
                logger.Log($"Raw response: {response.ResponseText}");
            }
        }
    }
}