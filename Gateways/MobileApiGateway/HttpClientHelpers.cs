using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileApiGateway
{
    public class HttpClientHelpers
    {
        private HttpContext _httpContext;
        private HttpClient _httpClient;
        private ILogger<HttpClientHelpers> _logger;

        public HttpClientHelpers(IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient,
            ILogger<HttpClientHelpers> logger)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CommandResult<T>> Process<T>(HttpResponseMessage responseMessage)
        {
            var jsonString = await responseMessage.Content.ReadAsStringAsync();
            if (responseMessage.IsSuccessStatusCode)
            {
                var objectified = JsonConvert.DeserializeObject<T>(jsonString);
                return new CommandResult<T>(objectified);
            }
            else
            {
                try
                {
                    var errorObjectified = JsonConvert.DeserializeObject<ErrorMessage>(jsonString);
                    return new CommandResult<T>(errorObjectified);
                }
                catch (Exception e)
                {
                    return new CommandResult<T>(new ErrorMessage
                    {
                        Message = "Internal server error",
                        ErrorCode = "INTERNAL.SERVER.ERROR",
                        StatusCode = responseMessage.StatusCode
                    });
                }
            }
        }

        public async Task<HttpRequestMessage> CreateAsync(
            HttpMethod method,
            string url,
            bool forwardUrlPars,
            bool forwardHeaders,
            // add new headers,
            Func<string, (string Content, string ContentType)> changeBody = null)
        {
            string finalUrl = forwardUrlPars ? url + _httpContext.Request.QueryString.ToUriComponent() : url;

            var request = new HttpRequestMessage(method, finalUrl);
            _httpContext.Request.QueryString.ToUriComponent();
            if (forwardHeaders)
            {
                foreach (var requestHeader in _httpContext.Request.Headers)
                {
                    if (requestHeader.Key.EqualsIC("Host")) continue;
                    request.Headers.TryAddWithoutValidation(requestHeader.Key, requestHeader.Value.AsEnumerable());
                }
            }

            if (changeBody is null)
            {
                return request;

            }

            var body = new StreamReader(_httpContext.Request.Body);
            //The modelbinder has already read the stream and need to reset the stream index
            body.BaseStream.Seek(0, SeekOrigin.Begin);
            var requestBody = await body.ReadToEndAsync();

            var (content, type) = changeBody(requestBody);
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType.MediaType = type;

            return request;
        }

        public async Task<HttpRequestMessage> CreateAsync(
            HttpContext context,
            HttpMethod method,
            string url,
            bool forwardUrlPars,
            bool forwardHeaders,
            // add new headers,
            string content, string contentType)
        {
            string finalUrl = forwardUrlPars ? url + context.Request.QueryString.ToUriComponent() : url;

            var request = new HttpRequestMessage(method, finalUrl);
            if (forwardHeaders)
            {
                foreach (var requestHeader in context.Request.Headers)
                {
                    if (requestHeader.Key.EqualsIC("Host")) continue;
                    request.Headers.TryAddWithoutValidation(requestHeader.Key, requestHeader.Value.AsEnumerable());
                }
            }

            request.Content = new StringContent(content);
            request.Content.Headers.ContentType.MediaType = contentType;

            return request;
        }


        public async Task<HttpRequestMessage> CreateAsync(
            HttpContext context,
            HttpMethod method,
            string url,
            bool forwardUrlPars,
            bool forwardHeaders,
            // add new headers,
            object @object)
        {
            var jsonString = JsonConvert.SerializeObject(@object);
            return await CreateAsync(context, method, url, forwardUrlPars, forwardHeaders, jsonString, "application/json");
        }

        public async Task<CommandResult<T>> SendAndProcess<T>(HttpRequestMessage request, string errorCode)
        {
            try
            {
                var response = await _httpClient.SendAsync(request);
                var objectResult = await Process<T>(response);

                return objectResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error While making a remote request");
                var message = new ErrorMessage
                {
                    ErrorCode = errorCode,
                    Message = "there were an error while trying to execute your request",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                };
                return new CommandResult<T>(message);
            }
        }


        public async Task<CommandResult<T>> CreateAndProcess<T>(HttpMethod method, string url, string errorCode)
        {
            var request = await CreateAsync(method, url, true, true, null);
            try
            {
                var response = await _httpClient.SendAsync(request);
                var objectResult = await Process<T>(response);

                return objectResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error While making a remote request");
                var message = new ErrorMessage
                {
                    ErrorCode = errorCode,
                    Message = "there were an error while trying to execute your request",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                };
                return new CommandResult<T>(message);
            }
        }
    }
}
