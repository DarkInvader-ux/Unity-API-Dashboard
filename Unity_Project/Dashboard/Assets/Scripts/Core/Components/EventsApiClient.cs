using System.Collections;
using System.Collections.Generic;
using Core.Configuration;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Core.Services.Events;
using Core.Testing;
using UnityEngine;
using IEventsService = Core.Interfaces.IEventsService;
using ILogger = Core.Interfaces.ILogger;

namespace Core.Components
{
    /// <summary>
    /// Main Unity component for Events API integration
    /// Serves as the composition root and public interface
    /// </summary>
    public class EventsApiClient : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ApiConfiguration apiConfig = new ApiConfiguration();
        
        [Header("Test Settings")]
        [SerializeField] private bool testOnStart = true;
        [SerializeField] private string testEventName = "Unity Test Event";
        [SerializeField] private bool enableDebugLogs = true;

        [Header("Advanced Test Settings")]
        [SerializeField] private int stressTestEventCount = 5;
        [SerializeField] private float stressTestDelay = 0.2f;
        
        [Header("Game Data")]
        [SerializeField] private GameDataConfig gameDataConfig;
        
        [Header("Game Event Data")]
        [SerializeField] private GameEventDataSo gameEventDto;


        // Dependencies (injected via composition)
        private ILogger logger;
        private IHttpClient httpClient;
        private IEventsService eventsService;
        private ApiTestRunner testRunner;

        // Properties for external access
        public ApiConfiguration Config => apiConfig;
        public IEventsService EventsService => eventsService;
        public ApiTestRunner TestRunner => testRunner;

        #region Unity Lifecycle

        void Awake()
        {
            InitializeDependencies();
            LogInitialization();
        }

        void Start()
        {
            if (testOnStart)
            {
                StartCoroutine(testRunner.RunFullTest(gameEventDto.ToDto()));
            }
        }

        void Update()
        {
            //HandleInput();
        }

        #endregion

        #region Dependency Injection
        
        /// <summary>
        /// Initializes all dependencies following dependency injection pattern
        /// </summary>
        private void InitializeDependencies()
        {
            // Create dependencies in proper order (dependencies first)
            logger = new UnityConsoleLogger("[EventsAPI]", enableDebugLogs);
            httpClient = new UnityHttpClient(logger);
            eventsService = new EventsService(httpClient, apiConfig, logger);
            testRunner = new ApiTestRunner(eventsService, logger);
        }

        /// <summary>
        /// Logs initialization information
        /// </summary>
        private void LogInitialization()
        {
            logger.Log($"EventsAPIClient initialized");
            logger.Log($"API URL: {apiConfig.EventsUrl}");
            logger.Log($"Debug logging: {(enableDebugLogs ? "Enabled" : "Disabled")}");
            logger.Log($"Test on start: {(testOnStart ? "Enabled" : "Disabled")}");
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
                logger.Log("Manual POST test triggered (P key)");
                StartCoroutine(testRunner.PostTestEvent());
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                logger.Log("Manual GET test triggered (G key)");
                StartCoroutine(testRunner.GetAllEvents());
            }
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                logger.Log("Manual full test triggered (T key)");
                StartCoroutine(testRunner.RunFullTest(gameEventDto.ToDto()));
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                logger.Log("Stress test triggered (S key)");
                StartCoroutine(testRunner.RunStressTest(stressTestEventCount, stressTestDelay));
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                logger.Log("Error handling test triggered (E key)");
                StartCoroutine(testRunner.RunErrorHandlingTest());
            }
        }

        #endregion

        #region Public API for UI/External Use
        
        /// <summary>
        /// Posts a test event - can be called from UI buttons
        /// </summary>
        public void PostEventFromUI() 
        {
            StartCoroutine(testRunner.PostTestEvent());
        }

        /// <summary>
        /// Gets all events - can be called from UI buttons
        /// </summary>
        public void GetEventsFromUI() 
        {
            StartCoroutine(testRunner.GetAllEvents());
        }

        /// <summary>
        /// Runs full test - can be called from UI buttons
        /// </summary>
        public void RunFullTestFromUI() 
        {
            StartCoroutine(testRunner.RunFullTest(gameEventDto.ToDto()));
        }

        /// <summary>
        /// Runs stress test - can be called from UI buttons
        /// </summary>
        public void RunStressTestFromUI()
        {
            StartCoroutine(testRunner.RunStressTest(stressTestEventCount, stressTestDelay));
        }

        /// <summary>
        /// Posts a custom event (for future gesture integration)
        /// </summary>
        /// <param name="eventName">Name of the event to post</param>
        public void PostCustomGameData()
        {
            if (gameDataConfig == null)
            {
                logger.LogError("No GameDataConfig assigned.");
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
                logger.LogError("Cannot update API URL: new URL is null or empty");
                return;
            }

            apiConfig.SetBaseUrl(newBaseUrl);
            logger.Log($"API base URL updated to: {newBaseUrl}");
            logger.Log($"Events endpoint is now: {apiConfig.EventsUrl}");
        }

        /// <summary>
        /// Updates the events endpoint at runtime
        /// </summary>
        /// <param name="newEndpoint">New endpoint path</param>
        public void UpdateEventsEndpoint(string newEndpoint)
        {
            if (string.IsNullOrEmpty(newEndpoint))
            {
                logger.LogError("Cannot update events endpoint: new endpoint is null or empty");
                return;
            }

            apiConfig.SetEventsEndpoint(newEndpoint);
            logger.Log($"Events endpoint updated to: {newEndpoint}");
            logger.Log($"Full events URL is now: {apiConfig.EventsUrl}");
        }

        /// <summary>
        /// Toggles debug logging at runtime
        /// </summary>
        public void ToggleDebugLogging()
        {
            enableDebugLogs = !enableDebugLogs;
            
            // Reinitialize logger with new setting
            logger = new UnityConsoleLogger("[EventsAPI]", enableDebugLogs);
            httpClient = new UnityHttpClient(logger);
            eventsService = new EventsService(httpClient, apiConfig, logger);
            testRunner = new ApiTestRunner(eventsService, logger);
            
            logger.Log($"Debug logging {(enableDebugLogs ? "enabled" : "disabled")}");
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
            
            logger.Log($"Stress test settings updated: {stressTestEventCount} events, {stressTestDelay}s delay");
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
            logger.Log("=== Current Configuration ===");
            logger.Log($"Base URL: {apiConfig.BaseUrl}");
            logger.Log($"Events Endpoint: {apiConfig.EventsEndpoint}");
            logger.Log($"Full Events URL: {apiConfig.EventsUrl}");
            logger.Log($"Timeout: {apiConfig.TimeoutSeconds}s");
            logger.Log($"Debug Logging: {enableDebugLogs}");
            logger.Log($"Test on Start: {testOnStart}");
            logger.Log($"Test Event Name: {testEventName}");
            logger.Log($"Stress Test Count: {stressTestEventCount}");
            logger.Log($"Stress Test Delay: {stressTestDelay}s");
            logger.Log("=============================");
        }

        /// <summary>
        /// Shows available keyboard shortcuts
        /// </summary>
        [ContextMenu("Show Keyboard Shortcuts")]
        public void ShowKeyboardShortcuts()
        {
            logger.Log("=== Keyboard Shortcuts ===");
            logger.Log("P - Post test event");
            logger.Log("G - Get all events");
            logger.Log("T - Run full test");
            logger.Log("S - Run stress test");
            logger.Log("E - Run error handling test");
            logger.Log("==========================");
        }

        #endregion
    }
}