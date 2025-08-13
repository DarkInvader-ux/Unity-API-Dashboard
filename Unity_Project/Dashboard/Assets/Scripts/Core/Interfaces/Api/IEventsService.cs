using System.Collections;
using Core.Data;
using Core.Models;

namespace Core.Interfaces
{
    /// <summary>
    /// Abstraction for events API operations
    /// </summary>
    public interface IEventsService
    {
        /// <summary>
        /// Posts an event to the API
        /// </summary>
        /// <param name="eventName">Name of the event to post</param>
        /// <returns>Coroutine for async execution</returns>
        IEnumerator PostEvent(GameEventDto eventName);
        
        /// <summary>
        /// Retrieves all events from the API
        /// </summary>
        /// <returns>Coroutine for async execution</returns>
        IEnumerator GetEvents();
    }
}