using System;
using System.Collections;
using Core.Interfaces;
using Core.Models; // ✅ Make sure GameEventDto is here
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

        public ApiTestRunner(IEventsService eventsService, ILogger logger)
        {
            this.eventsService = eventsService ?? throw new ArgumentNullException(nameof(eventsService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerator RunFullTest(GameEventDto testEventName)
        {
            logger.Log("=== Starting Unity ↔ API Test ===");

            logger.Log("Testing POST operation...");
            yield return eventsService.PostEvent(testEventName);

            yield return new WaitForSeconds(0.5f);

            logger.Log("Testing GET operation...");
            yield return eventsService.GetEvents();

            logger.Log("=== API Test Complete ===");
        }

        public IEnumerator PostTestEvent(string eventName = null)
        {
            var testName = eventName ?? GenerateTestEventName();
            logger.Log($"Posting test event: {testName}");
            yield return eventsService.PostEvent(CreateTestEventDto(testName));
        }

        public IEnumerator GetAllEvents()
        {
            logger.Log("Retrieving all events...");
            yield return eventsService.GetEvents();
        }

        public IEnumerator RunStressTest(int eventCount = 5, float delayBetweenRequests = 0.2f)
        {
            logger.Log($"=== Starting Stress Test ({eventCount} events) ===");

            for (int i = 1; i <= eventCount; i++)
            {
                var eventName = $"Stress Test Event {i}/{eventCount}";
                logger.Log($"Posting event {i}/{eventCount}...");
                yield return eventsService.PostEvent(CreateTestEventDto(eventName));

                if (i < eventCount && delayBetweenRequests > 0)
                {
                    yield return new WaitForSeconds(delayBetweenRequests);
                }
            }

            yield return new WaitForSeconds(1f);
            logger.Log("Retrieving all events after stress test...");
            yield return eventsService.GetEvents();

            logger.Log("=== Stress Test Complete ===");
        }

        public IEnumerator RunErrorHandlingTest()
        {
            logger.Log("=== Starting Error Handling Test ===");

            logger.Log("Testing empty event name...");
            yield return eventsService.PostEvent(CreateTestEventDto(""));

            yield return new WaitForSeconds(0.5f);

            logger.Log("Testing null event name...");
            yield return eventsService.PostEvent(CreateTestEventDto(null));

            yield return new WaitForSeconds(0.5f);

            logger.Log("Testing very long event name...");
            var longEventName = new string('A', 1000);
            yield return eventsService.PostEvent(CreateTestEventDto(longEventName));

            logger.Log("=== Error Handling Test Complete ===");
        }

        private string GenerateTestEventName()
        {
            return $"Manual Test - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        /// <summary>
        /// Creates a GameEventDto with minimal valid fields for testing
        /// </summary>
        private GameEventDto CreateTestEventDto(string eventName)
        {
            return new GameEventDto
            {
                playerId = "TestPlayer01",
                eventType = eventName,
                position = new PositionDto { x = 0, y = 0, z = 0 }
            };
        }
    }
}
