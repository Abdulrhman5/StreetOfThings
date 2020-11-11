using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services
{
    class ObjectViewsService : IObjectViewsService
    {

        private IUserDataManager _userDataManager;

        private ILogger<ObjectViewsService> _logger;

        private IRepository<(int, Guid, DateTime), ObjectView> _viewsRepo;

        public ObjectViewsService(
            IUserDataManager userDataManager,
            ILogger<ObjectViewsService> logger, IRepository<(int, Guid, DateTime), ObjectView> viewsRepo)
        {
            _userDataManager = userDataManager;
            _logger = logger;
            _viewsRepo = viewsRepo;
        }

        public async Task AddView(int objectId)
        {
            var login = await _userDataManager.AddCurrentUserIfNeeded();

            if (login.Item1 is null)
            {
                throw new NotSupportedException("This user is not logged in");
            }

            var view = new ObjectView
            {
                LoginId = login.Item1.LoginId,
                ObjectId = objectId,
                ViewedAtUtc = DateTime.UtcNow,
            };

            try
            {
                _viewsRepo.Add(view);
                await _viewsRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("There is an error while trying to add impressions");
            }
        }
    }
}
