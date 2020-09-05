using AutoMapper;
using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MobileApiGateway.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileApiGateway.Services
{
    public class CatalogAggregator
    {
        private HttpClient _httpClient;

        private HttpContext _httpContext;

        private HttpClientHelpers _responseProcessor;

        private IConfiguration _configs;

        private ILogger<CatalogService> _logger;

        private UserService _userService;

        private CurrentUserCredentialsGetter _credentialsGetter;

        private IMapper _mapper;
        public CatalogAggregator(HttpClient httpClient, IHttpContextAccessor httpContextAccessor,
            HttpClientHelpers responseProcessor,
            IConfiguration configs,
            ILogger<CatalogService> logger,
            UserService userService,
            CurrentUserCredentialsGetter credentialsGetter,
            IMapper mapper)
        {
            _httpClient = httpClient;
            _httpContext = httpContextAccessor.HttpContext;
            _responseProcessor = responseProcessor;
            _configs = configs;
            _logger = logger;
            _userService = userService;
            _credentialsGetter = credentialsGetter;
            _mapper = mapper;
        }

        public async Task<CommandResult<List<DownstreamObjectDto>>> AggregateObjects()
        {
            var request = await _responseProcessor.CreateAsync(_httpContext, HttpMethod.Get, $"{_configs["Servers:Catalog"]}/api/object/list", true, true, changeBody: null);
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


        public async Task<CommandResult<List<DownstreamObjectDtoV1_1>>> AggregateObjectsV1_1()
        {
            var request = await _responseProcessor.CreateAsync(_httpContext, HttpMethod.Get, $"{_configs["Servers:Catalog"]}/api/object/v1.1/list", true, true, changeBody: null);
            try
            {
                var response = await _httpClient.SendAsync(request);
                var objectResult = await _responseProcessor.Process<List<UpstreamObjectDtoV1_1>>(response);
                if (!objectResult.IsSuccessful)
                {
                    return new CommandResult<List<DownstreamObjectDtoV1_1>>(objectResult.Error);
                }

                var originalUserIds = objectResult.Result.Select(o => o.OwnerId).ToList();
                var users = await _userService.GetUsersAsync(originalUserIds);
                var callerUserId = _credentialsGetter.GetCuurentUser().UserId;
                var distances = await _userService.CalculateUsersDistances(callerUserId, users.Select(u => u.Id).ToList());
                return new CommandResult<List<DownstreamObjectDtoV1_1>>(ReplaceUserIdWithUserV1_1(objectResult.Result, users, distances));
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
                return new CommandResult<List<DownstreamObjectDtoV1_1>>(message);
            }
        }
        private List<DownstreamObjectDto> ReplaceUserIdWithUser(List<UpstreamObjectDto> objects, List<UserDto> users)
        {
            var downstreamObjects = _mapper.Map<List<DownstreamObjectDto>>(objects);
            downstreamObjects.ForEach(downObject =>
            {
                var upObject = objects.Find(o => o.Id == downObject.Id);
                downObject.Owner = users.FirstOrDefault(u => u.Id == upObject.OwnerId);
            });
            downstreamObjects.RemoveAll(o => o.Owner is null);
            return downstreamObjects;
        }

        private List<DownstreamObjectDtoV1_1> ReplaceUserIdWithUserV1_1(List<UpstreamObjectDtoV1_1> objects, List<UserDto> users, List<(double? distance, string userId)> distances)
        {
            var downstreamObjects = _mapper.Map<List<DownstreamObjectDtoV1_1>>(objects);
            downstreamObjects.ForEach(downObject =>
            {
                var upObject = objects.Find(o => o.Id == downObject.Id);
                downObject.Owner = users.FirstOrDefault(u => u.Id == upObject.OwnerId);
            });
            downstreamObjects.RemoveAll(o => o.Owner is null);
            return downstreamObjects;
        }
    }
}
