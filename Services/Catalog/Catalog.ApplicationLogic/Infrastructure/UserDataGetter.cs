using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.Infrastructure
{
    public class UserDataGetter 
    {
        private RestClient _restClient;

        private IConfiguration _configruations;
        public UserDataGetter(IConfiguration configuration)
        {
            _configruations = configuration;
            var authorization = _configruations["Services:Authorization"];
            _restClient = new RestClient(authorization);
        }

        public async Task<UserDataDto?> GetUserDataByToken(string token)
        {
            var request = new RestRequest("connect/userInfo");
            request.AddParameter("Authorization", token,ParameterType.HttpHeader);

            var response = await _restClient.ExecuteAsync(request);
            if(response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserialized = JsonConvert.DeserializeObject<UserDataDto>(response.Content);
                return deserialized;
            }
            else
            {
                Log.Error($"Couldn't connect or an error occurred while trying to get the user information, Token: {token}, response: {response}");
                return null;
            }
        }
    }

    public class UserDataDto
    {
        public string Sub { get; set; }

        public string UserId => Sub;

        [JsonProperty("NormalizedName")]
        public string UserName { get; set; }

        public string TokenId { get; set; }

        [JsonProperty("ProfilePhoto")]
        public string PictureUrl { get; set; }
    }
}
