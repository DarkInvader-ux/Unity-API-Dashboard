using Core.Configuration.Api;
using Core.Data;
using Core.Interfaces.Api;
using Core.Models;
using Core.Services;
using Core.Services.Events;
using Core.Testing;
using UnityEngine;
using Zenject;
using IEventsService = Core.Interfaces.IEventsService;
using ILogger = Core.Interfaces.Api.ILogger;

namespace Core.Components.Api
{
    public class EventsApiClient : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ApiConfiguration apiConfig = new ApiConfiguration();
        
        [Header("Test Settings")]
        [SerializeField] private bool testOnStart = true;
        [SerializeField] private bool enableDebugLogs = true;

        [Header("Advanced Test Settings")]
        [SerializeField] private int stressTestEventCount = 5;
        [SerializeField] private float stressTestDelay = 0.2f;
        
        [Header("Game Data")]
        [SerializeField] private GameDataConfig gameDataConfig;
        
        [Header("Game Event Data")]
        [SerializeField] private GameEventDataSo gameEventDto;

        private ILogger _logger;
        private IHttpClient _httpClient;
        private IEventsService _eventsService;
        private ApiTestRunner _testRunner;
        [Inject]
        public void Construct(ILogger logger, IHttpClient httpClient, IEventsService eventsService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _eventsService = eventsService;
            _testRunner = new ApiTestRunner(_eventsService, _logger);
        }

        #region Monobehaviour
        void Start()
        {
            if (testOnStart)
            {
                StartCoroutine(_testRunner.RunFullTest(gameEventDto.ToDto()));
            }
        }

        void Update()
        {
            //HandleInput();
        }
        #endregion
        
        #region Input Handling
        
        /// <summary>
        /// Handles keyboard input for testing
        /// </summary>
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _logger.Log("Manual POST test triggered (P key)");
                StartCoroutine(_testRunner.PostTestEvent());
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                _logger.Log("Manual GET test triggered (G key)");
                StartCoroutine(_testRunner.GetAllEvents());
            }
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                _logger.Log("Manual full test triggered (T key)");
                StartCoroutine(_testRunner.RunFullTest(gameEventDto.ToDto()));
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _logger.Log("Stress test triggered (S key)");
                StartCoroutine(_testRunner.RunStressTest(stressTestEventCount, stressTestDelay));
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _logger.Log("Error handling test triggered (E key)");
                StartCoroutine(_testRunner.RunErrorHandlingTest());
            }
        }

        #endregion

        #region Public API for UI/External Use
        
        /// <summary>
        /// Posts a test event - can be called from UI buttons
        /// </summary>
        public void PostEventFromUI() 
        {
            StartCoroutine(_testRunner.PostTestEvent());
        }

        /// <summary>
        /// Gets all events - can be called from UI buttons
        /// </summary>
        public void GetEventsFromUI() 
        {
            StartCoroutine(_testRunner.GetAllEvents());
        }

        /// <summary>
        /// Runs full test - can be called from UI buttons
        /// </summary>
        public void RunFullTestFromUI() 
        {
            StartCoroutine(_testRunner.RunFullTest(gameEventDto.ToDto()));
        }

        /// <summary>
        /// Runs stress test - can be called from UI buttons
        /// </summary>
        public void RunStressTestFromUI()
        {
            StartCoroutine(_testRunner.RunStressTest(stressTestEventCount, stressTestDelay));
        }

        /// <summary>
        /// Posts a custom event (for future gesture integration)
        /// </summary>
        /// <param name="eventName">Name of the event to post</param>
        public void PostCustomGameData()
        {
            if (gameDataConfig == null)
            {
                _logger.LogError("No GameDataConfig assigned.");
                return;
            }

            var dto = gameDataConfig.GameDataDto();
            //StartCoroutine(eventsService.PostGameData(dto)); 
        }


        /// <summary>
        /// Posts a custom event with additional metadata
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="metadata">Additional event metadata</param>
        public void PostCustomEventWithMetadata(string eventName, string metadata = null)
        {
            var fullEventName = string.IsNullOrEmpty(metadata) 
                ? eventName 
                : $"{eventName} | {metadata}";
            
            //PostCustomEvent(fullEventName);
        }
        // public IEnumerator PostGameData(GameDataDTO gameData)
        // {
        //     var json = JsonUtility.ToJson(gameData);
        //     yield return httpClient.Post(apiConfig.EventsUrl, json, "application/json");
        // }

        #endregion

        #region Runtime Configuration

        /// <summary>
        /// Updates the API base URL at runtime
        /// </summary>
        /// <param name="newBaseUrl">New base URL</param>
        public void UpdateApiUrl(string newBaseUrl)
        {
            if (string.IsNullOrEmpty(newBaseUrl))
            {
                _logger.LogError("Cannot update API URL: new URL is null or empty");
                return;
            }

            apiConfig.SetBaseUrl(newBaseUrl);
            _logger.Log($"API base URL updated to: {newBaseUrl}");
            _logger.Log($"Events endpoint is now: {apiConfig.EventsUrl}");
        }

        /// <summary>
        /// Updates the events endpoint at runtime
        /// </summary>
        /// <param name="newEndpoint">New endpoint path</param>
        public void UpdateEventsEndpoint(string newEndpoint)
        {
            if (string.IsNullOrEmpty(newEndpoint))
            {
                _logger.LogError("Cannot update events endpoint: new endpoint is null or empty");
                return;
            }

            apiConfig.SetEventsEndpoint(newEndpoint);
            _logger.Log($"Events endpoint updated to: {newEndpoint}");
            _logger.Log($"Full events URL is now: {apiConfig.EventsUrl}");
        }

        /// <summary>
        /// Toggles debug logging at runtime
        /// </summary>
        public void ToggleDebugLogging()
        {
            enableDebugLogs = !enableDebugLogs;
            
            // Reinitialize logger with new setting
            _logger = new UnityConsoleLogger("[EventsAPI]", enableDebugLogs);
            _httpClient = new UnityHttpClient(_logger);
            _eventsService = new EventsService(_httpClient, apiConfig, _logger);
            _testRunner = new ApiTestRunner(_eventsService, _logger);
            
            _logger.Log($"Debug logging {(enableDebugLogs ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Updates stress test parameters
        /// </summary>
        /// <param name="eventCount">Number of events for stress test</param>
        /// <param name="delay">Delay between requests</param>
        public void UpdateStressTestSettings(int eventCount, float delay)
        {
            stressTestEventCount = Mathf.Max(1, eventCount);
            stressTestDelay = Mathf.Max(0f, delay);
            
            _logger.Log($"Stress test settings updated: {stressTestEventCount} events, {stressTestDelay}s delay");
        }

        #endregion

        #region Unity Editor Helpers

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            // Ensure reasonable values in the inspector
            stressTestEventCount = Mathf.Max(1, stressTestEventCount);
            stressTestDelay = Mathf.Max(0f, stressTestDelay);
        }

        #endregion

        #region Debug Info

        /// <summary>
        /// Displays current configuration info (useful for debugging)
        /// </summary>
        [ContextMenu("Show Configuration Info")]
        public void ShowConfigurationInfo()
        {
            _logger.Log("=== Current Configuration ===");
            _logger.Log($"Base URL: {apiConfig.BaseUrl}");
            _logger.Log($"Events Endpoint: {apiConfig.EventsEndpoint}");
            _logger.Log($"Full Events URL: {apiConfig.EventsUrl}");
            _logger.Log($"Timeout: {apiConfig.TimeoutSeconds}s");
            _logger.Log($"Debug Logging: {enableDebugLogs}");
            _logger.Log($"Test on Start: {testOnStart}");
            _logger.Log($"Stress Test Count: {stressTestEventCount}");
            _logger.Log($"Stress Test Delay: {stressTestDelay}s");
            _logger.Log("=============================");
        }

        /// <summary>
        /// Shows available keyboard shortcuts
        /// </summary>
        [ContextMenu("Show Keyboard Shortcuts")]
        public void ShowKeyboardShortcuts()
        {
            _logger.Log("=== Keyboard Shortcuts ===");
            _logger.Log("P - Post test event");
            _logger.Log("G - Get all events");
            _logger.Log("T - Run full test");
            _logger.Log("S - Run stress test");
            _logger.Log("E - Run error handling test");
            _logger.Log("==========================");
        }

        #endregion
    }
}