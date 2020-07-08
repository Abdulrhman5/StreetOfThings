using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transaction.Models;

namespace Transaction.BusinessLogic.Infrastructure
{
    class RemoteObjectGetter : IRemotlyObjectGetter
    {
        private IRestClient _restClient;

        private IConfiguration _configurations;

        private CurrentUserCredentialsGetter _credentialsGetter;
        private RemoteObjectGetter(IConfiguration configurations)
        {
            _configurations = configurations;
            var catalog = _configurations["Services:Catalog"];
            _restClient = new RestClient(catalog);
        }

        public async Task<OfferedObject> GetObject(ulong objectId)
        {
            var request = new RestRequest("object/byId"+objectId);
            request.AddParameter("Authorization", _credentialsGetter.GetCuurentUser().AccessToken, ParameterType.HttpHeader);

            var response = await _restClient.ExecuteAsync(request);
            if (response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserialized = JsonConvert.DeserializeObject<ObjectDto>(response.Content);
                return new OfferedObject
                {
                    HourlyCharge = null,
                    OriginalObjectId = deserialized.Id,
                    OwnerUserId = Guid.Parse(deserialized.OwnerId),
                    ShouldReturn = deserialized.Type == TransactionType.Free
                };
            }
            else
            {
                Log.Error($"There were a problem while trying to get offered object The response is {response.Content}");
                return null;
            }

        }
    }

    class ObjectDto
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

    public enum TransactionType
    {
        Lending,
        Free
    }

}
