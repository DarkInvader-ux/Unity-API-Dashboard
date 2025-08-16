using System;
using System.Collections;
using Core.Configuration.Api;
using Core.Data;
using Core.Interfaces;
using Core.Interfaces.Api;
using Core.Models;
using UnityEngine;
using Zenject;
using ILogger = Core.Interfaces.Api.ILogger;

namespace Core.Services.Events
{
    public class EventsService : IEventsService
    {
        private readonly IHttpClient _httpClient;
        private readonly ApiConfiguration _config;
        private readonly ILogger _logger;

        [Inject]
        public EventsService(IHttpClient httpClient, ApiConfiguration config, ILogger logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IEnumerator PostEvent(GameEventDto gameEventDto)
        {
            if (gameEventDto == null)
            {
                _logger.LogError("Event data cannot be null");
                yield break;
            }

            var request = new HttpRequest<GameEventDto>
            {
                Url = _config.EventsUrl,
                Method = HttpMethod.POST,
                Data = gameEventDto,
                OnComplete = response => OnEventPosted(gameEventDto.eventType, response),
                OnError = error => _logger.LogError($"Failed to post event '{gameEventDto.eventType}': {error}")
            };

            yield return _httpClient.SendRequest(request);
        }

        public IEnumerator GetEvents()
        {
            var request = new HttpRequest<object>
            {
                Url = _config.EventsUrl,
                Method = HttpMethod.GET,
                Data = null,
                OnComplete = OnEventsReceived,
                OnError = error => _logger.LogError($"Failed to get events: {error}")
            };

            yield return _httpClient.SendRequest(request);
        }

        private void OnEventPosted(string eventName, HttpResponse response)
        {
            _logger.LogSuccess($"Event '{eventName}' posted successfully");
            _logger.Log($"Server response: {response.ResponseText}");
        }

        private void OnEventsReceived(HttpResponse response)
        {
            try
            {
                // Wrap the response in an object since JsonUtility needs an object wrapper
                var wrappedJson = $"{{\"events\":{response.ResponseText}}}";
                var eventsResponse = JsonUtility.FromJson<EventsResponse>(wrappedJson);
                
                if (eventsResponse?.events != null)
                {
                    _logger.LogSuccess($"Retrieved {eventsResponse.events.Length} events:");
                    for (int i = 0; i < eventsResponse.events.Length; i++)
                    {
                        _logger.Log($"  [{i}] {eventsResponse.events[i]}");
                    }
                }
                else
                {
                    _logger.Log("No events found or empty response");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to parse events response: {e.Message}");
                _logger.Log($"Raw response: {response.ResponseText}");
            }
        }
    }
}