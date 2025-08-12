using System.Collections;
using Core.Models;

namespace Core.Interfaces
{
    /// <summary>
    /// Abstraction for HTTP client operations
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// Sends an HTTP request asynchronously
        /// </summary>
        /// <typeparam name="T">Type of request data</typeparam>
        /// <param name="request">The HTTP request to send</param>
        /// <returns>Coroutine for async execution</returns>
        IEnumerator SendRequest<T>(HttpRequest<T> request);
    }
}