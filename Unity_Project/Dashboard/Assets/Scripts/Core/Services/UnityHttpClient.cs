using System;
using System.Collections;
using Core.Interfaces.Api;
using Core.Models;
using Newtonsoft.Json;
using UnityEngine.Networking;
using ILogger = Core.Interfaces.Api.ILogger;
using Newtonsoft.Json.Linq;

namespace Core.Services
{
    /// <summary>
    /// Unity-specific HTTP client implementation
    /// </summary>
    public class UnityHttpClient : IHttpClient
    {
        private readonly ILogger logger;

        /// <summary>
        /// Creates a new Unity HTTP client
        /// </summary>
        /// <param name="logger">Logger instance for request logging</param>
        /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
        public UnityHttpClient(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends an HTTP request using Unity's networking system
        /// </summary>
        /// <typeparam name="T">Type of request data</typeparam>
        /// <param name="request">The HTTP request to send</param>
        /// <returns>Coroutine for async execution</returns>
        /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
        /// <exception cref="NotImplementedException">Thrown for unsupported HTTP methods</exception>
        public IEnumerator SendRequest<T>(HttpRequest<T> request)
        {
            if (request == null) 
                throw new ArgumentNullException(nameof(request));
            
            logger.Log($"Sending {request.Method} request to: {request.Url}");

            yield return request.Method switch
            {
                HttpMethod.GET => SendGetRequest(request),
                HttpMethod.POST => SendPostRequest(request),
                HttpMethod.PUT => SendPutRequest(request),
                HttpMethod.DELETE => SendDeleteRequest(request),
                _ => throw new NotImplementedException($"HTTP method {request.Method} not implemented")
            };
        }

        /// <summary>
        /// Sends a GET request
        /// </summary>
        private IEnumerator SendGetRequest<T>(HttpRequest<T> request)
        {
            using var webRequest = UnityWebRequest.Get(request.Url);
            yield return webRequest.SendWebRequest();
            HandleResponse(webRequest, request);
        }

        /// <summary>
        /// Sends a POST request
        /// </summary>
        private IEnumerator SendPostRequest<T>(HttpRequest<T> request)
        {
            var jsonData = SerializeData(request.Data);
            logger.Log($"POST data: {jsonData}");

            using var webRequest = UnityWebRequest.PostWwwForm(request.Url, "");
            webRequest.uploadHandler = new UploadHandlerRaw(
                System.Text.Encoding.UTF8.GetBytes(jsonData));
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            yield return webRequest.SendWebRequest();
            HandleResponse(webRequest, request);
        }

        /// <summary>
        /// Sends a PUT request
        /// </summary>
        private IEnumerator SendPutRequest<T>(HttpRequest<T> request)
        {
            var jsonData = SerializeData(request.Data);
            logger.Log($"PUT data: {jsonData}");

            using var webRequest = UnityWebRequest.Put(request.Url, jsonData);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            yield return webRequest.SendWebRequest();
            HandleResponse(webRequest, request);
        }

        /// <summary>
        /// Sends a DELETE request
        /// </summary>
        private IEnumerator SendDeleteRequest<T>(HttpRequest<T> request)
        {
            using var webRequest = UnityWebRequest.Delete(request.Url);
            yield return webRequest.SendWebRequest();
            HandleResponse(webRequest, request);
        }

        /// <summary>
        /// Handles the HTTP response and invokes appropriate callbacks
        /// </summary>
        private void HandleResponse<T>(UnityWebRequest webRequest, HttpRequest<T> request)
        {
            var response = new HttpResponse
            {
                StatusCode = webRequest.responseCode,
                ResponseText = webRequest.downloadHandler?.text ?? "",
                IsSuccess = webRequest.result == UnityWebRequest.Result.Success,
                Error = webRequest.error
            };

            if (response.IsSuccess)
            {
                logger.LogSuccess($"Request completed. Status: {response.StatusCode}");
                logger.Log($"Response: {response.ResponseText}");
                request.OnComplete?.Invoke(response);
            }
            else
            {
                // Log additional details for 4xx/5xx errors
                logger.LogError($"Request failed. Status: {response.StatusCode}, Error: {response.Error}");
        
                if (!string.IsNullOrWhiteSpace(response.ResponseText))
                {
                    logger.LogError($"Server error details: {response.ResponseText}");
                }

                // Specific case: Bad Request// Add at top

                if (response.StatusCode == 400)
                {
                    try
                    {
                        var errorDetails = JObject.Parse(response.ResponseText);
                        logger.LogError($"400 Bad Request details:\n{errorDetails.ToString()}");
                    }
                    catch
                    {
                        logger.LogError("Could not parse 400 response as JSON. Raw body:\n" + response.ResponseText);
                    }
                }


                request.OnError?.Invoke(response.Error ?? response.ResponseText);
            }
        }


        /// <summary>
        /// Serializes request data to JSON
        /// </summary>
        private string SerializeData<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}