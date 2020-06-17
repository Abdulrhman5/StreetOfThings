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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MobileApiGateway.Services
{
    public class CatalogService
    {
        private HttpClient _httpClient;

        private HttpContext _httpContext;

        private HttpClientHelpers _responseProcessor;

        private IConfiguration _configs;

        private ILogger<CatalogService> _logger;
        public CatalogService(
            IHttpContextAccessor contextAccessor,
            IConfiguration configs,
            HttpClientHelpers clientHelper,
            ILogger<CatalogService> logger)
        {
            _httpClient = new HttpClient();
            _httpContext = contextAccessor.HttpContext;
            _configs = configs;
            _logger = logger;
            _responseProcessor = clientHelper;
        }

        public async Task<CommandResult<List<DownstreamObjectDto>>> AggregateObjects()
        {
            var request = await _responseProcessor.CreateAsync(_httpContext, HttpMethod.Get, $"{_configs["Servers:Catalog"]}/api/object/list",true, true, changeBody: null);
            try
            {
                var response = await _httpClient.SendAsync(request);
                var objectResult = await _responseProcessor.Process<List<UpstreamObjectDto>>(response);
                if (!objectResult.IsSuccessful)
                {
                    return new CommandResult<List<DownstreamObjectDto>>(objectResult.Error);
                }

                var originalUserIds = objectResult.Result.Select(o => o.OwnerId).ToList();

                var userRequest = await _responseProcessor.CreateAsync(_httpContext, HttpMethod.Post, $"{_configs["Servers:Identity"]}/api/users/listFromIds",true, true, originalUserIds);
                var userResponse = await _httpClient.SendAsync(userRequest);
                var result = await _responseProcessor.Process<List<UserDto>>(userResponse);

                if (!result.IsSuccessful)
                {
                    new CommandResult<List<DownstreamObjectDto>>(result.Error);
                }

                var responseString = ReplaceUserIdWithUser(objectResult.Result , result.Result);
                return new CommandResult<List<DownstreamObjectDto>>(responseString);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error When getting list of objects");
                var message = new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.LIST.ERROR",
                    Message = "there were an error while trying to execute your request",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                };
                return new CommandResult<List<DownstreamObjectDto>>(message);
            }
        }

        private List<DownstreamObjectDto> ReplaceUserIdWithUser(List<UpstreamObjectDto> objects, List<UserDto> users)
        {
            var downStreamObjects = new List<DownstreamObjectDto>();
            foreach(var @object in objects)
            {
                downStreamObjects.Add(new DownstreamObjectDto
                {
                    CountOfImpressions = @object.CountOfImpressions,
                    CountOfViews = @object.CountOfViews,
                    Description = @object.Description,
                    Id = @object.Id,
                    Name = @object.Name,
                    Owner = users?.FirstOrDefault(u => u.Id == @object.OwnerId),
                    Photos = @object.Photos,
                    Rating = @object.Rating,
                    Tags = @object.Tags,
                    Type = @object.Type
                });
            }

            return downStreamObjects;
        }
    }
}
