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

        private IDistanceCalcultaor _distanceCalcultaor;
        public UserService(IUserGetter userGetter, IDistanceCalcultaor distanceCalcultaor)
        {
            _userGetter = userGetter;
            _distanceCalcultaor = distanceCalcultaor;
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
                Username = user.Username
            }));
            return model;
        }

        public override async Task<DistancesResponse> CalculateDistance(CalculateDistanceRequest request, ServerCallContext context)
        {
            var userIds = request.UserIds.ToList();
            var theUserId = request.TheUserId;

            var distances = await _distanceCalcultaor.CalculateDistancesAsync(theUserId, userIds);

            var distanceResponse = new DistancesResponse();
            distances.ForEach(distance => distanceResponse.Distances.Add(new DistanceModel
            {
                Distance = distance.distance,
                UserId = distance.userId
            }));

            return distanceResponse;
        }
    }
}
