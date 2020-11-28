using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;

namespace Transaction.Core.Behaviours
{
    class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        private readonly IUserDataManager _userDataManager;
        public LoggingBehaviour(ILogger<TRequest> logger, IUserDataManager userDataManager)
        {
            _logger = logger;
            _userDataManager = userDataManager;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _userDataManager.GetCurrentUser()?.UserId ?? string.Empty;

            _logger.LogInformation("Transaction.Service Request: {Name} {@UserId} {@Request}",
                requestName, userId, request);
        }

    }
}
