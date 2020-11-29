using Infrastructure.Emails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Infrastructure.Test
{
    public class EmailSender
    {
        [Fact]
        public void SendEmail()
        {
            var email = new Email()
            {
                HtmlBody = "<html> <p> Hello </p> </html>",
                Subject = "Hello there",
                TextBody = "Hello there, how are you"
            };

            var configurations = new Mock<IConfiguration>();
            configurations.Setup(c => c[It.IsAny<string>()])
                .Returns<string>(par =>
                par switch
                {
                    "EmailConfigs:Email" => "abdulrhman.emailconfirmation@gmail.com",
                    "EmailConfigs:Password" => "1qa2ws#ED",
                    "EmailConfigs:Host" => "smtp.gmail.com",
                    "EmailConfigs:Port" => "587",
                    _ => null
                }
            );

            var logger = new Mock<ILogger<Infrastructure.Emails.EmailSender>>();
            var emailSender = new Infrastructure.Emails.EmailSender(logger.Object, configurations.Object);

            emailSender.SendEmailByEmailAsync("abdulrhman.m.alrifai@gmail.com",email).GetAwaiter().GetResult();
        }
    }
}
