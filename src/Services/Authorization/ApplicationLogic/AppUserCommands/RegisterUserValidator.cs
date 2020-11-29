using CommonLibrary;
using Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ApplicationLogic.AppUserCommands
{
    public interface IRegisterUserValidator
    {
        CommandResult<Models.AppUser> ValidateRegistrationOfNewUser(RegisterUserDto user);
    }

    public class RegisterUserValidator : IRegisterUserValidator
    {
        public CommandResult<Models.AppUser> ValidateRegistrationOfNewUser(RegisterUserDto user)
        {
            if(user == null)
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.INVALID",
                    Message = "Please fill the required data",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            if (user.Email.IsNullOrEmpty())
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.EMAIL.NULL",
                    Message = "Please fill the email field",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            if (!IsValidEmail(user.Email))
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGILTER.EMAIL.INVALID",
                    Message = "The Email feild does not represent an email",
                    StatusCode = HttpStatusCode.BadRequest

                });
            }

            if (user.Username.IsNullOrEmpty())
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.USERNAME.NULL",
                    Message = "Please fill the user name feild",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            if(user.Username.Length <= 5 || user.Username.Length >= 200)
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.USERNAME.INVALID",
                    Message = "The user name must be longer than 5 and less than 200",
                    StatusCode = HttpStatusCode.BadRequest

                });
            }

            
            if (user.Gender.IsNullOrEmpty())
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.GENDER.NULL",
                    Message = "Please fill the user gender",
                    StatusCode = HttpStatusCode.BadRequest

                });
            }

            if(!Enum.TryParse(user.Gender, true, out Gender gender))
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.GENDER.INVALID",
                    Message = "Please fill the gender with valid data",
                    StatusCode = HttpStatusCode.BadRequest

                });
            }

            if (user.PhoneNumber.IsNullOrEmpty())
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.PHONE_NUMBER.NULL",
                    Message = "Please fill the phone number feild",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            if (IsValidPhoneNumber(user.PhoneNumber))
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.PHONE_NUMBER.INVALID",
                    Message = "Please fill the phone number with valid data",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            if (user.Password.IsNullOrEmpty())
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.PASSWORD.NULL",
                    Message = "Please fill the password feild",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            if (user.PasswordConfirmation.IsNullOrEmpty())
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.PASSWORD_CONFIRMATION.NULL",
                    Message = "Please fill the password confirmation feild",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            if(user.Password != user.PasswordConfirmation)
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = "USER.REGISTER.PASSWORD_CONFIRMATION.INVALID",
                    Message = "The password and confirmation are not the same",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            return new CommandResult<Models.AppUser>(new Models.AppUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = user.Email,
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = user.PhoneNumber,
                UserName = user.Email,
                NormalizedName = user.Username,
                Gender = gender,
            });
        }

        bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.Match(phoneNumber, @"^\+\(([0-9]{1,3})\)\s([0-9]{3})\-([0-9]{3})\-([0-9]{3})$").Success;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
