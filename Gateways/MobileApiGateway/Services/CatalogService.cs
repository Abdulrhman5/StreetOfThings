using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading.Channels;
using Grpc.Core;
using AuthorizationService;

namespace MobileApiGateway.Services
{
    public class CatalogService
    {
        private HttpClient _httpClient;

        private HttpResponseMessageConverter _responseMessageConverter;

        private HttpContext _httpContext;

        private ResponseProcessor _responseProcessor;
        public CatalogService(
            IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            _responseMessageConverter = new HttpResponseMessageConverter();
            _httpContext = contextAccessor.HttpContext;
            _responseProcessor = new ResponseProcessor();
        }

        public async Task<IActionResult> AggregateObjects()
        {
            var channel = new Grpc.Core.Channel("localhost:20000", ChannelCredentials.Insecure);
            var usersClient = new UserDirectory.UserDirectoryClient(channel);
            var queryString = _httpContext.Request.QueryString.ToUriComponent();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:20001/api/object/list" + queryString);

            foreach (var requestHeader in _httpContext.Request.Headers)
            {
                request.Headers.TryAddWithoutValidation(requestHeader.Key, requestHeader.Value.AsEnumerable());
            }

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    await _responseMessageConverter.ConvertAndCopyResponse(_httpContext, response);
                    return new EmptyResult();
                }

                var originalUserIds = GetUserIds(await response.Content.ReadAsStringAsync());

                var userRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:20000/api/users/listFromIds");
                var usersIdsString = JsonConvert.SerializeObject(originalUserIds);
                userRequest.Content = new StringContent(usersIdsString);
                userRequest.Content.Headers.ContentType.MediaType = "application/json";
                var userResponse = await _httpClient.SendAsync(userRequest);
                var result = await _responseProcessor.Process<List<UserDto>>(userResponse);
                if (!result.IsSuccessful)
                {
                    return new JsonResult(result.Error)
                    {
                        StatusCode = (int)result.Error.StatusCode
                    };
                }

                var responseString = ReplaceUserIdWithUser(await response.Content.ReadAsStringAsync(), result.Result);
                return new JsonResult(responseString)
                {
                    StatusCode = 200
                };
            }
            catch(Exception e)
            {
                var message = new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.LIST.ERROR",
                    Message = "there were an error while trying to execute your request",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                };
                return new JsonResult(message)
                {
                    StatusCode = 500
                };
            }
        }

        private List<string> GetUserIds(string response)
        {
            var objectified = JsonConvert.DeserializeObject<List<Dictionary<string,object>>>(response);
            var usersIds = new List<string>();
            foreach(var @object in objectified)
            {
                usersIds.Add(@object["ownerId"] as string);
            }
            return usersIds;
        }

        private object ReplaceUserIdWithUser(string response, List<UserDto> users)
        {
            var objectified = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);
            foreach(var @object in objectified)
            {
                var user = users.FirstOrDefault(u => u.Id == (@object["ownerId"] as string));
                @object.Remove("ownerId");
                if (user == null)
                {
                    @object.Add("owner", null);
                }
                else
                {
                    @object.Add("owner", user);
                }
            }

            return objectified;
        }
    }


}
