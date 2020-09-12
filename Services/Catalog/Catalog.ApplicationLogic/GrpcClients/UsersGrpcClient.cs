using AuthorizationService.Grpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.GrpcClients
{
    class UsersGrpcClient
    {
        private UsersGrpc.UsersGrpcClient _grpcClient;

        public UsersGrpcClient(UsersGrpc.UsersGrpcClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<LoginInformationDto> GetLoginInformation(string tokenId)
        {
            var request = new UserLoginInformationRequest
            {
                TokenId = tokenId
            };

            var result = await _grpcClient.GetUserLoginInformationAsync(request);
            var dto = new LoginInformationDto
            {
                Longitude = result.Longitude,
                Latitude = result.Latitude,
                UserId = result.UserId,
                Username = result.Username,
                Email = result.Email,
                LoggedAtUtc = result.LoggedAtUtc.ToDateTime(),
                TokenId = result.TokenId,
            };
            return dto;
        }
    }

    public class LoginInformationDto
    {
        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public DateTime LoggedAtUtc { get; set; }

        public string TokenId { get; set; }

    }
}
