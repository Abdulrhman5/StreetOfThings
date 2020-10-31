using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.Service.Infrastructure
{
    public interface ITransactionTokenManager
    {
        Task<TransactionToken> GenerateToken(Guid transactionId, TokenType type);

        Task<TransactionToken> GetToken(string token);

        Task InvalidateToken(string token);
    }
    class TransactionTokenManager : ITransactionTokenManager
    {
        private IRepository<Guid, TransactionToken> _tokensRepo;

        private IAlphaNumericStringGenerator _stringGenerator;

        private UserDataManager _userDataManager;
        public TransactionTokenManager(IRepository<Guid, TransactionToken> tokensRepo, IAlphaNumericStringGenerator stringGenerator, UserDataManager userDataManager)
        {
            _tokensRepo = tokensRepo;
            _stringGenerator = stringGenerator;
            _userDataManager = userDataManager;
        }

        public async Task<TransactionToken> GenerateToken(Guid transactionId, TokenType type)
        {
            var(login, user)= await _userDataManager.AddCurrentUserIfNeeded();

            var tokenString = _stringGenerator.GenerateIdentifier(100, CharsInToken.CapitalSmallNumeric_);
            var token = new TransactionToken
            {
                TransactionTokenId = Guid.NewGuid(),
                IssuedAtUtc = DateTime.UtcNow,
                UseAfterUtc = DateTime.UtcNow,
                UseBeforeUtc = DateTime.UtcNow.AddDays(6),
                Status = TokenStatus.Ok,
                Token = tokenString,
                Type = type,
                IssuerLoginId = login.LoginId
            };

            if (type == TokenType.Receiving)
            {
                token.RegistrationId = transactionId;
            }
            else if (type == TokenType.Returning)
            {
                token.ReceivingId = transactionId;
            }

            var model = _tokensRepo.Add(token);
            await _tokensRepo.SaveChangesAsync();
            return model;
        }

        public async Task<TransactionToken> GetToken(string token)
        {
            throw new NotImplementedException();
        }

        public async Task InvalidateToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
