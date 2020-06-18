using Catalog.ApplicationLogic.ObjectQueries;
using Catalog.DataAccessLayer;
using Catalog.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.Infrastructure
{
    class ObjectImpressionsManager : IObjectImpressionsManager
    {
        private CurrentUserCredentialsGetter _credintialsGetter;

        private UserDataManager _userDataManager;

        private ILogger<ObjectImpressionsManager> _logger;

        private IRepository<(int, Guid, DateTime), ObjectImpression> _impressionsRepo;
        public ObjectImpressionsManager(
            CurrentUserCredentialsGetter credentialsGetter,
            UserDataManager userDataManager,
            IRepository<(int, Guid, DateTime), ObjectImpression> impressionRepo,
            ILogger<ObjectImpressionsManager> logger)
        {
            _credintialsGetter = credentialsGetter;
            _userDataManager = userDataManager;
            _impressionsRepo = impressionRepo;
            _logger = logger;
        }

        public async Task AddImpressions(List<int> objectsIds)
        {
            var login = await _userDataManager.AddCurrentUserIfNeeded();

            if (login.Item1 is null)
            {
                throw new NotSupportedException("This user is not logged in");
            }

            var imps = objectsIds.Select(oi => new ObjectImpression
            {
                LoginId = login.Item1.LoginId,
                ObjectId = oi,
                ViewedAtUtc = DateTime.UtcNow
            }).ToList();

            try
            {
                _impressionsRepo.AddRange(imps);
                await _impressionsRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("There is an error while trying to add impressions");
            }
        }

        public async Task AddImpressions(List<ObjectDto> objects)
        {
            await AddImpressions(objects.Select(o => o.Id).ToList());
        }
    }
}
