using AdministrationGateway.Services.TransactionServices;
using CommonLibrary;
using Grpc.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdministrationGateway.Services
{
    public class ObjectAggregator
    {
        private HttpClient _httpClient;

        private HttpContext _httpContext;

        private HttpClientHelpers _responseProcessor;

        private IConfiguration _configs;

        private ILogger<ObjectAggregator> _logger;

        private UserService _userService;

        public ObjectAggregator(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, HttpClientHelpers responseProcessor, IConfiguration configs, ILogger<ObjectAggregator> logger, UserService userService)
        {
            _httpClient = httpClient;
            _httpContext = httpContextAccessor.HttpContext;
            _responseProcessor = responseProcessor;
            _configs = configs;
            _logger = logger;
            _userService = userService;
        }

        public async Task<CommandResult<List<DownstreamObjectDto>>> AggregateObjects()
        {
            var request = await _responseProcessor.CreateAsync(_httpContext, HttpMethod.Get, $"{_configs["Services:Catalog"]}/api/object/allObjects", true, true, changeBody: null);
            try
            {
                var response = await _httpClient.SendAsync(request);
                var objectResult = await _responseProcessor.Process<List<UpstreamObjectDto>>(response);
                if (!objectResult.IsSuccessful)
                {
                    return new CommandResult<List<DownstreamObjectDto>>(objectResult.Error);
                }

                var originalUserIds = objectResult.Result.Select(o => o.OwnerId).ToList();
                var users = await _userService.GetUsersAsync(originalUserIds);
                return new CommandResult<List<DownstreamObjectDto>>(ReplaceUserIdWithUser(objectResult.Result, users));
            }
            catch (Exception e)
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
            foreach (var @object in objects)
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
                downStreamObjects.RemoveAll(o => o.Owner is null);
            }

            return downStreamObjects;
        }




        class UpstreamObjectDto
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public int CountOfViews { get; set; }

            public int CountOfImpressions { get; set; }

            public float? Rating { get; set; }

            public string OwnerId { get; set; }

            public List<string> Photos { get; set; }

            public List<string> Tags { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public TransactionType Type { get; set; }
        }

        public class DownstreamObjectDto
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public int CountOfViews { get; set; }

            public int CountOfImpressions { get; set; }

            public float? Rating { get; set; }

            public List<string> Photos { get; set; }

            public List<string> Tags { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public TransactionType Type { get; set; }

            public UserDto Owner { get; set; }

        }
        public enum TransactionType
        {
            Lending,
            Free
        }

    }
}
