using System;
using System.Collections;
using Core.Interfaces.Api;
using Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using Zenject;
using ILogger = Core.Interfaces.Api.ILogger;

namespace Core.Services.Events
{
    public class UnityHttpClient : IHttpClient
    {
        private readonly ILogger _logger;
        [Inject]
        public UnityHttpClient(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IEnumerator SendRequest<T>(HttpRequest<T> request)
        {
            if (request == null) 
                throw new ArgumentNullException(nameof(request));
            
            _logger.Log($"Sending {request.Method} request to: {request.Url}");

            yield return request.Method switch
            {
                HttpMethod.GET => SendGetRequest(request),
                HttpMethod.POST => SendPostRequest(request),
                HttpMethod.PUT => SendPutRequest(request),
                HttpMethod.DELETE => SendDeleteRequest(request),
                _ => throw new NotImplementedException($"HTTP method {request.Method} not implemented")
            };
        }

        private IEnumerator SendGetRequest<T>(HttpRequest<T> request)
        {
            using var webRequest = UnityWebRequest.Get(request.Url);
            yield return webRequest.SendWebRequest();
            HandleResponse(webRequest, request);
        }

        private IEnumerator SendPostRequest<T>(HttpRequest<T> request)
        {
            var jsonData = SerializeData(request.Data);
            _logger.Log($"POST data: {jsonData}");

            using var webRequest = UnityWebRequest.PostWwwForm(request.Url, "");
            webRequest.uploadHandler = new UploadHandlerRaw(
                System.Text.Encoding.UTF8.GetBytes(jsonData));
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            yield return webRequest.SendWebRequest();
            HandleResponse(webRequest, request);
        }
        private IEnumerator SendPutRequest<T>(HttpRequest<T> request)
        {
            var jsonData = SerializeData(request.Data);
            _logger.Log($"PUT data: {jsonData}");

            using var webRequest = UnityWebRequest.Put(request.Url, jsonData);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            yield return webRequest.SendWebRequest();
            HandleResponse(webRequest, request);
        }
        private IEnumerator SendDeleteRequest<T>(HttpRequest<T> request)
        {
            using var webRequest = UnityWebRequest.Delete(request.Url);
            yield return webRequest.SendWebRequest();
            HandleResponse(webRequest, request);
        }

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
                _logger.LogSuccess($"Request completed. Status: {response.StatusCode}");
                _logger.Log($"Response: {response.ResponseText}");
                request.OnComplete?.Invoke(response);
            }
            else
            {
                // Log additional details for 4xx/5xx errors
                _logger.LogError($"Request failed. Status: {response.StatusCode}, Error: {response.Error}");
        
                if (!string.IsNullOrWhiteSpace(response.ResponseText))
                {
                    _logger.LogError($"Server error details: {response.ResponseText}");
                }

                // Specific case: Bad Request// Add at top

                if (response.StatusCode == 400)
                {
                    try
                    {
                        var errorDetails = JObject.Parse(response.ResponseText);
                        _logger.LogError($"400 Bad Request details:\n{errorDetails.ToString()}");
                    }
                    catch
                    {
                        _logger.LogError("Could not parse 400 response as JSON. Raw body:\n" + response.ResponseText);
                    }
                }


                request.OnError?.Invoke(response.Error ?? response.ResponseText);
            }
        }
        private string SerializeData<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}