using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileApiGateway
{
    public class ResponseProcessor
    {
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

        public async Task<HttpRequestMessage> SendAsync(
            HttpContext context,
            HttpMethod method,
            string url,
            bool forwardHeaders,
            // add new headers,
            Func<string, (string Content, string ContentType)> changeBody = null
            )
        {
            var request = new HttpRequestMessage(method, url);
            if (forwardHeaders)
            {
                foreach (var requestHeader in context.Request.Headers)
                {
                    request.Headers.TryAddWithoutValidation(requestHeader.Key, requestHeader.Value.AsEnumerable());
                }
            }

            if (changeBody is null)
            {
                return request;

            }

            var body = new StreamReader(context.Request.Body);
            //The modelbinder has already read the stream and need to reset the stream index
            body.BaseStream.Seek(0, SeekOrigin.Begin);
            var requestBody = await body.ReadToEndAsync();

            var (content, type) = changeBody(requestBody);
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType.MediaType = type;

            return request;
        }

        public async Task<HttpRequestMessage> SendAsync(
            HttpContext context,
            HttpMethod method,
            string url,
            bool forwardHeaders,
            // add new headers,
            string content, string contentType
            )
        {
            var request = new HttpRequestMessage(method, url);
            if (forwardHeaders)
            {
                foreach (var requestHeader in context.Request.Headers)
                {
                    request.Headers.TryAddWithoutValidation(requestHeader.Key, requestHeader.Value.AsEnumerable());
                }
            }

            request.Content = new StringContent(content);
            request.Content.Headers.ContentType.MediaType = contentType;

            return request;
        }


        public async Task<HttpRequestMessage> SendAsync(
            HttpContext context,
            HttpMethod method,
            string url,
            bool forwardHeaders,
            // add new headers,
            object @object
            )
        {
            var jsonString = JsonConvert.SerializeObject(@object);
            return await SendAsync(context, method, url, forwardHeaders, jsonString, "application/json");
        }


    }
}
