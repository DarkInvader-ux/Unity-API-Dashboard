using System;

namespace Core.Models
{
    /// <summary>
    /// HTTP method enumeration
    /// </summary>
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    /// <summary>
    /// Generic HTTP request model
    /// </summary>
    /// <typeparam name="T">Type of request data</typeparam>
    [Serializable]
    public class HttpRequest<T>
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public T Data { get; set; }
        public Action<HttpResponse> OnComplete { get; set; }
        public Action<string> OnError { get; set; }
    }

    /// <summary>
    /// HTTP response model
    /// </summary>
    [Serializable]
    public class HttpResponse
    {
        public long StatusCode { get; set; }
        public string ResponseText { get; set; }
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Events API response model
    /// </summary>
    [Serializable]
    public class EventsResponse
    {
        public string[] events;
    }
}