using System;
using System.Collections;
using Core.Interfaces;
using UnityEngine;
using ILogger = Core.Interfaces.ILogger;


namespace Core.Testing
{
    /// <summary>
    /// Orchestrates API testing operations
    /// </summary>
    public class ApiTestRunner
    {
        private readonly IEventsService eventsService;
        private readonly ILogger logger;

        /// <summary>
        /// Creates a new API test runner
        /// </summary>
        /// <param name="eventsService">Events service to test</param>
        /// <param name="logger">Logger for test operations</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
        public ApiTestRunner(IEventsService eventsService, ILogger logger)
        {
            this.eventsService = eventsService ?? throw new ArgumentNullException(nameof(eventsService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Runs a complete API test (POST then GET)
        /// </summary>
        /// <param name="testEventName">Name of the test event to post</param>
        /// <returns>Coroutine for async execution</returns>
        public IEnumerator RunFullTest(string testEventName)
        {
            logger.Log("=== Starting Unity ↔ API Test ===");

            // Test POST operation
            logger.Log("Testing POST operation...");
            yield return eventsService.PostEvent(testEventName);
            
            // Wait between requests to allow server processing
            yield return new WaitForSeconds(0.5f);
            
            // Test GET operation
            logger.Log("Testing GET operation...");
            yield return eventsService.GetEvents();
            
            logger.Log("=== API Test Complete ===");
        }

        /// <summary>
        /// Posts a test event with optional custom name
        /// </summary>
        /// <param name="eventName">Custom event name, or null for auto-generated</param>
        /// <returns>Coroutine for async execution</returns>
        public IEnumerator PostTestEvent(string eventName = null)
        {
            var testName = eventName ?? GenerateTestEventName();
            logger.Log($"Posting test event: {testName}");
            yield return eventsService.PostEvent(testName);
        }

        /// <summary>
        /// Retrieves all events from the API
        /// </summary>
        /// <returns>Coroutine for async execution</returns>
        public IEnumerator GetAllEvents()
        {
            logger.Log("Retrieving all events...");
            yield return eventsService.GetEvents();
        }

        /// <summary>
        /// Runs a stress test by posting multiple events
        /// </summary>
        /// <param name="eventCount">Number of events to post</param>
        /// <param name="delayBetweenRequests">Delay between each request in seconds</param>
        /// <returns>Coroutine for async execution</returns>
        public IEnumerator RunStressTest(int eventCount = 5, float delayBetweenRequests = 0.2f)
        {
            logger.Log($"=== Starting Stress Test ({eventCount} events) ===");

            for (int i = 1; i <= eventCount; i++)
            {
                var eventName = $"Stress Test Event {i}/{eventCount}";
                logger.Log($"Posting event {i}/{eventCount}...");
                yield return eventsService.PostEvent(eventName);
                
                if (i < eventCount && delayBetweenRequests > 0)
                {
                    yield return new WaitForSeconds(delayBetweenRequests);
                }
            }

            // Get all events to see the results
            yield return new WaitForSeconds(1f);
            logger.Log("Retrieving all events after stress test...");
            yield return eventsService.GetEvents();
            
            logger.Log("=== Stress Test Complete ===");
        }

        /// <summary>
        /// Tests error handling by posting invalid data
        /// </summary>
        /// <returns>Coroutine for async execution</returns>
        public IEnumerator RunErrorHandlingTest()
        {
            logger.Log("=== Starting Error Handling Test ===");

            // Test empty event name
            logger.Log("Testing empty event name...");
            yield return eventsService.PostEvent("");
            
            yield return new WaitForSeconds(0.5f);

            // Test null event name
            logger.Log("Testing null event name...");
            yield return eventsService.PostEvent(null);
            
            yield return new WaitForSeconds(0.5f);

            // Test very long event name
            logger.Log("Testing very long event name...");
            var longEventName = new string('A', 1000);
            yield return eventsService.PostEvent(longEventName);
            
            logger.Log("=== Error Handling Test Complete ===");
        }

        /// <summary>
        /// Generates a unique test event name with timestamp
        /// </summary>
        /// <returns>Generated event name</returns>
        private string GenerateTestEventName()
        {
            return $"Manual Test - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }
    }
}