using CommonLibrary;
using DataAccessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.UserConfirmations
{
    public interface IEmailConfirmationManager
    {
        public Task<CommandResult> ConfirmEmailAsync(string email, string confirmationCode);

        public Task<string> GenerateConfirmationCodeAsync(string userId);

        public Task<string> GenerateConfirmationCodeAsync(AppUser usre);
    }

    class EmailConfirmation : IEmailConfirmationManager
    {
        private UserManager<AppUser> _userManager;

        private IStringGenerator _stringGenerator;

        private IRepository<Guid, ConfirmationToken> _confirmationToken;

        private IRepository<string, AppUser> _userRepo;

        private ILogger<EmailConfirmation> _logger;
        public EmailConfirmation(UserManager<AppUser> userManager,
            IRepository<Guid,ConfirmationToken> confirmationTokenRepo,
            IRepository<string, AppUser> userRepo,
            ILogger <EmailConfirmation> logger,
            IStringGenerator stringGenerator
            )
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _logger = logger;
            _stringGenerator = stringGenerator;
            _confirmationToken = confirmationTokenRepo;
        }

        public async Task<CommandResult> ConfirmEmailAsync(string email, string confirmationCode)
        {
            if (email.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage()
                {
                    ErrorCode = "USER.CONFIRMATION.EMAIL.NULL",
                    Message = "Please fill the email feild",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (confirmationCode.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage()
                {
                    ErrorCode = "USER.CONFIRMATION.CODE.NULL",
                    Message = "Please fill the confirmation code",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new CommandResult(new ErrorMessage()
                {
                    ErrorCode = "USER.CONFIRMATION.EMAIL.INCORRRECT",
                    Message = "The email you are trying to confirm is not an account",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (user.EmailConfirmed)
            {
                return new CommandResult(new ErrorMessage()
                {
                    ErrorCode = "USER.CONFIRMATION.EMAIL.CONFIRMED",
                    Message = "The email you are trying to confirm is already confirmed",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            var tokensForUser = (from ec in _confirmationToken.Table
                                 where ec.ConfirmationType == ConfirmationToken.ConfirmationCodeTypeEmail && ec.User.Email.ToLower() == email.ToLower()
                                 orderby ec.ExpiresAtUtc
                                 descending
                                 select ec).ToList();


            var lastTokenSent = tokensForUser.FirstOrDefault();

            // if there is a token
            // if the token is the same
            // and it is not expired
            if (lastTokenSent is object &&
                lastTokenSent.ConfirmationCode.ToUpper() == confirmationCode.ToUpper() &&
                lastTokenSent.ExpiresAtUtc >= DateTime.UtcNow)
            {
                try
                {
                    foreach (var token in tokensForUser)
                    {
                        _confirmationToken.Delete(token);
                    }
                    await _confirmationToken.SaveChangesAsync().ConfigureAwait(false);


                    user.EmailConfirmed = true;
                    await _userRepo.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError("There where an error while trying to delete the confirmations token and update the state of the user", e.Message);
                }
            }
            else
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "USER.CONFIRMATION.EMAIL.FAULTY",
                    Message = "Incorrect confirmation code",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }
            return null;
        }

        public async Task<string> GenerateConfirmationCodeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            var code = _stringGenerator.GenerateIdentifier(6, CharsInToken.CapitalNumeric);
            var newCode = new ConfirmationToken
            {
                ConfirmationCode = code,
                ConfirmationTokenId = Guid.NewGuid(),
                ConfirmationType = ConfirmationToken.ConfirmationCodeTypeEmail,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30),
                IssuedAtUtc = DateTime.UtcNow,
                UserId = userId
            };
            _confirmationToken.Add(newCode);
            await _confirmationToken.SaveChangesAsync();

            return code;
        }

        public async Task<string> GenerateConfirmationCodeAsync(AppUser user)
        {
            if (user == null) return null;

            var code = _stringGenerator.GenerateIdentifier(6, CharsInToken.CapitalNumeric);
            var newCode = new ConfirmationToken
            {
                ConfirmationCode = code,
                ConfirmationTokenId = Guid.NewGuid(),
                ConfirmationType = ConfirmationToken.ConfirmationCodeTypeEmail,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30),
                IssuedAtUtc = DateTime.UtcNow,
                UserId = user.Id
            };
            _confirmationToken.Add(newCode);
            await _confirmationToken.SaveChangesAsync();
            return code;
        }
    }
}
