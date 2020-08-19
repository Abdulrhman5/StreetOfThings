using ApplicationLogic.AppUserQueries;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;
using static AuthorizationService.Grpc.UsersGrpc;

namespace AuthorizationService.Grpc
{
    public class UserService : UsersGrpcBase
    {
        private IUserGetter _userGetter;
        public UserService(IUserGetter userGetter)
        {
            _userGetter = userGetter;

        }

        public override async Task<UsersModel> GetUsersData(UsersIdsModel request, ServerCallContext context)
        {
            var users = (await _userGetter.GetUserByIdsAsync(request.UsersIds.ToList())).ToList();
            var model = new UsersModel();

            model.Users.AddRange(users.Select(user => new UserModel
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PictureUrl = user.PictureUrl,
                Username = user.PictureUrl
            }));
            return model;
        }
    }
}
