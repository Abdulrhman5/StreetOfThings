using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AuthorizationService.Grpc.UsersGrpc;
using AuthorizationService.Grpc;
using CommonLibrary;

namespace MobileApiGateway.Services
{
    public class UserService
    {
        private UsersGrpc.UsersGrpcClient _grpcClient;

        public UserService(UsersGrpcClient usersGrpcClient)
        {
            _grpcClient = usersGrpcClient;
        }

        public async Task<List<UserDto>> GetUsersAsync(List<string> usersIds)
        {
            if (usersIds.IsNullOrEmpty())
            {
                return new List<UserDto>();
            }
            var usersIdsModel = new UsersIdsModel();
            usersIds.ForEach(uid => usersIdsModel.UsersIds.Add(uid));

            var users = await _grpcClient.GetUsersDataAsync(usersIdsModel);

            var usersDtos = users.Users.Select(umodel => new UserDto
            {
                Email = umodel.Email,
                Id = umodel.Id,
                Name = umodel.Name,
                PictureUrl = umodel.PictureUrl,
                Username = umodel.Username
            }).ToList();

            return usersDtos;
        }
    }
}
