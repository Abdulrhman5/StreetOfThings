using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AuthorizationService.Grpc.UsersGrpc;
using AuthorizationService.Grpc;
using CommonLibrary;
using AdministrationGateway.Services.TransactionServices;
using Microsoft.Extensions.Configuration;
using Grpc.Net.Client;

namespace AdministrationGateway.Services.TransactionServices
{
    public class UserService
    {
        private UsersGrpc.UsersGrpcClient _grpcClient;

        private IConfiguration _configs;
        public UserService(IConfiguration configs)
        {

            _configs = configs;
            var channel = GrpcChannel.ForAddress(_configs["Services:Grpc:Authorization"]);
            _grpcClient = new UsersGrpcClient(channel);
        }

        public async Task<List<TransactionUserDto>> GetUsersAsync(List<string> usersIds)
        {
            if (usersIds.IsNullOrEmpty())
            {
                return new List<TransactionUserDto>();
            }
            var usersIdsModel = new UsersIdsModel();
            usersIds.ForEach(uid => usersIdsModel.UsersIds.Add(uid));

            var users = await _grpcClient.GetUsersDataAsync(usersIdsModel);

            var usersDtos = users.Users.Select(umodel => new TransactionUserDto
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
